using Microsoft.Extensions.Configuration;
using Scripters.Regula.Platform.Billing.Application.Internal.OutboundServices;
using Scripters.Regula.Platform.Billing.Domain.Errors;
using Scripters.Regula.Platform.Billing.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.Shared.Application.Model;
using Stripe;

namespace Scripters.Regula.Platform.Billing.Infrastructure.ExternalServices.StripeGateway;

/// <summary>
///     Adaptador / capa anticorrupción hacia Stripe. Es el único lugar de todo
///     el bounded context Billing que tiene "using Stripe;" — traduce los tipos
///     del SDK (Stripe.Event, Stripe.Subscription, etc.) a los DTOs neutrales
///     que Application conoce (CheckoutResult, SubscriptionWebhookEvent), y
///     convierte cualquier StripeException en un Result.Failure en vez de
///     dejarla propagar hacia capas que no deberían saber que Stripe existe.
/// </summary>
public class StripeService : IStripeService
{
    private readonly string _priceId;
    private readonly string _webhookSecret;

    public StripeService(IConfiguration configuration)
    {
        var secretKey = configuration["Stripe:SecretKey"];
        if (string.IsNullOrWhiteSpace(secretKey))
            throw new InvalidOperationException(
                "Stripe:SecretKey is not configured. Add it to appsettings.Development.json (never commit it).");

        StripeConfiguration.ApiKey = secretKey;

        _priceId = configuration["Stripe:PriceId"]
            ?? throw new InvalidOperationException("Stripe:PriceId is not configured.");

        _webhookSecret = configuration["Stripe:WebhookSecret"]
            ?? throw new InvalidOperationException("Stripe:WebhookSecret is not configured.");
    }

    public async Task<Result<CheckoutResult>> CreateOrRetrieveSubscriptionAsync(
        string username,
        long userId,
        string? existingCustomerId,
        string? existingSubscriptionId)
    {
        try
        {
            // Si ya hay una suscripción en Stripe (activa o pendiente de pago), la
            // reusamos en vez de crear otra — evita duplicar el cobro si el usuario
            // recarga la pantalla de pago.
            if (!string.IsNullOrEmpty(existingSubscriptionId))
            {
                var existingSubscription = await new SubscriptionService().GetAsync(
                    existingSubscriptionId,
                    new SubscriptionGetOptions { Expand = ["latest_invoice.payment_intent"] });

                if (existingSubscription.Status is "active" or "incomplete")
                {
                    var reusedSecret = existingSubscription.LatestInvoice?.PaymentIntent?.ClientSecret;

                    return Result<CheckoutResult>.Success(new CheckoutResult(
                        existingSubscription.CustomerId,
                        existingSubscription.Id,
                        reusedSecret,
                        MapStatus(existingSubscription.Status)));
                }
            }

            var customerId = existingCustomerId;

            if (string.IsNullOrEmpty(customerId))
            {
                var customer = await new CustomerService().CreateAsync(new CustomerCreateOptions
                {
                    // Iam.User no tiene email todavía — usamos el username como referencia
                    // visible en el Dashboard de Stripe, y el user_id en metadata para cruzarlo.
                    Description = $"Regula user #{userId} ({username})",
                    Metadata = new Dictionary<string, string> { { "regula_user_id", userId.ToString() } },
                });
                customerId = customer.Id;
            }

            var subscription = await new SubscriptionService().CreateAsync(new SubscriptionCreateOptions
            {
                Customer = customerId,
                Items = [new SubscriptionItemOptions { Price = _priceId }],
                // default_incomplete: crea la suscripción en estado "incomplete" y devuelve
                // el PaymentIntent del primer cobro sin confirmar — el frontend lo confirma
                // con Stripe.js (VueStripePaymentElement).
                PaymentBehavior = "default_incomplete",
                Expand = ["latest_invoice.payment_intent"],
                Metadata = new Dictionary<string, string> { { "regula_user_id", userId.ToString() } },
            });

            var clientSecret = subscription.LatestInvoice?.PaymentIntent?.ClientSecret;

            return Result<CheckoutResult>.Success(new CheckoutResult(
                customerId, subscription.Id, clientSecret, MapStatus(subscription.Status)));
        }
        catch (StripeException e)
        {
            return Result<CheckoutResult>.Failure(BillingErrors.StripeOperationFailed, e.Message);
        }
    }

    public Result<SubscriptionWebhookEvent> ParseWebhookEvent(string json, string signatureHeader)
    {
        try
        {
            // throwOnApiVersionMismatch: false — tu cuenta de Stripe manda eventos con una
            // versión de API más nueva que la que espera Stripe.net 47.4.0 por defecto.
            // Es el fix que el propio mensaje de error de Stripe recomienda. Riesgo real:
            // algún campo puede deserializar distinto entre versiones — el más sensible es
            // CurrentPeriodEnd más abajo (ver el comentario ahí).
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                signatureHeader,
                _webhookSecret,
                throwOnApiVersionMismatch: false);

            switch (stripeEvent.Type)
            {
                case "customer.subscription.created":
                case "customer.subscription.updated":
                {
                    if (stripeEvent.Data.Object is not global::Stripe.Subscription sub)
                        return Result<SubscriptionWebhookEvent>.Success(UnhandledEvent());

                    // CurrentPeriodEnd vive en Subscription en esta versión del SDK, pero tu
                    // cuenta ya usa una versión de API donde Stripe simplemente no manda ese
                    // campo ahí — Stripe.net lo deserializa como "vacío" (época Unix,
                    // 1970-01-01) en vez de null. Lo normalizamos a null explícito: es mejor
                    // no guardar una fecha, que guardar una fecha falsa.
                    var currentPeriodEnd = sub.CurrentPeriodEnd == default || sub.CurrentPeriodEnd.Year <= 1970
                        ? (DateTime?)null
                        : sub.CurrentPeriodEnd;

                    return Result<SubscriptionWebhookEvent>.Success(new SubscriptionWebhookEvent(
                        EWebhookEventType.SubscriptionUpserted,
                        sub.Id,
                        sub.CustomerId,
                        MapStatus(sub.Status),
                        currentPeriodEnd));
                }
                case "customer.subscription.deleted":
                {
                    if (stripeEvent.Data.Object is not global::Stripe.Subscription sub)
                        return Result<SubscriptionWebhookEvent>.Success(UnhandledEvent());

                    return Result<SubscriptionWebhookEvent>.Success(new SubscriptionWebhookEvent(
                        EWebhookEventType.SubscriptionCanceled,
                        sub.Id,
                        sub.CustomerId,
                        ESubscriptionStatus.Canceled,
                        null));
                }
                default:
                    return Result<SubscriptionWebhookEvent>.Success(UnhandledEvent());
            }
        }
        catch (StripeException e)
        {
            return Result<SubscriptionWebhookEvent>.Failure(BillingErrors.InvalidWebhookSignature, e.Message);
        }
    }

    private static SubscriptionWebhookEvent UnhandledEvent() =>
        new(EWebhookEventType.Unhandled, null, null, ESubscriptionStatus.None, null);

    /// <summary>Único lugar que traduce el vocabulario de Stripe (strings) al nuestro (enum).</summary>
    private static ESubscriptionStatus MapStatus(string stripeStatus) => stripeStatus switch
    {
        "active" => ESubscriptionStatus.Active,
        "past_due" => ESubscriptionStatus.PastDue,
        "canceled" or "unpaid" => ESubscriptionStatus.Canceled,
        "incomplete" or "incomplete_expired" => ESubscriptionStatus.Incomplete,
        _ => ESubscriptionStatus.Incomplete,
    };
}