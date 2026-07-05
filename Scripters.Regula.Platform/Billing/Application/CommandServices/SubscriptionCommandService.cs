using Scripters.Regula.Platform.Billing.Application.CommandServices;
using Scripters.Regula.Platform.Billing.Application.Internal.OutboundServices;
using Scripters.Regula.Platform.Billing.Domain.Model.Aggregates;
using Scripters.Regula.Platform.Billing.Domain.Model.Commands;
using Scripters.Regula.Platform.Billing.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.Billing.Domain.Repositories;
using Scripters.Regula.Platform.Shared.Application.Model;
using Scripters.Regula.Platform.Shared.Domain.Repositories;

namespace Scripters.Regula.Platform.Billing.Application.Internal.CommandServices;

/// <summary>
///     Puramente de aplicación: no tiene ni un solo "using Stripe;". Todo lo que
///     sabe de Stripe le llega ya traducido por IStripeService (CheckoutResult,
///     SubscriptionWebhookEvent) — si mañana cambia la pasarela de pago, esta
///     clase no se entera.
/// </summary>
public class SubscriptionCommandService(
    ISubscriptionRepository subscriptionRepository,
    IStripeService stripeService,
    IUnitOfWork unitOfWork) : ISubscriptionCommandService
{
    public async Task<Result<string?>> Handle(CreateCheckoutCommand command, CancellationToken cancellationToken)
    {
        var existing = await subscriptionRepository.FindByUserIdAsync(command.UserId, cancellationToken);

        if (existing is { Status: ESubscriptionStatus.Active })
            return Result<string?>.Success(null);

        var checkoutResult = await stripeService.CreateOrRetrieveSubscriptionAsync(
            command.Username,
            command.UserId,
            existing?.StripeCustomerId,
            existing?.StripeSubscriptionId);

        if (checkoutResult.IsFailure)
            return Result<string?>.Failure(checkoutResult.Error!, checkoutResult.Message);

        var checkout = checkoutResult.Value!;

        if (existing == null)
        {
            existing = new Subscription(command.UserId, checkout.CustomerId);
            await subscriptionRepository.AddAsync(existing, cancellationToken);
        }

        existing.AttachStripeSubscription(checkout.SubscriptionId, checkout.Status, null);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result<string?>.Success(checkout.ClientSecret);
    }

    public async Task<Result> HandleWebhook(string json, string signatureHeader, CancellationToken cancellationToken)
    {
        var parsed = stripeService.ParseWebhookEvent(json, signatureHeader);

        if (parsed.IsFailure)
            return Result.Failure(parsed.Error!, parsed.Message);

        var webhookEvent = parsed.Value!;

        switch (webhookEvent.EventType)
        {
            case EWebhookEventType.SubscriptionUpserted:
            {
                var local = await subscriptionRepository.FindByStripeSubscriptionIdAsync(webhookEvent.StripeSubscriptionId!, cancellationToken)
                    ?? await subscriptionRepository.FindByStripeCustomerIdAsync(webhookEvent.StripeCustomerId!, cancellationToken);

                if (local != null)
                {
                    local.AttachStripeSubscription(webhookEvent.StripeSubscriptionId!, webhookEvent.Status, webhookEvent.CurrentPeriodEnd);
                    await unitOfWork.CompleteAsync(cancellationToken);
                }

                break;
            }
            case EWebhookEventType.SubscriptionCanceled:
            {
                var local = await subscriptionRepository.FindByStripeSubscriptionIdAsync(webhookEvent.StripeSubscriptionId!, cancellationToken);

                if (local != null)
                {
                    local.UpdateStatus(ESubscriptionStatus.Canceled, null);
                    await unitOfWork.CompleteAsync(cancellationToken);
                }

                break;
            }
            case EWebhookEventType.Unhandled:
                break;
        }

        return Result.Success();
    }
}