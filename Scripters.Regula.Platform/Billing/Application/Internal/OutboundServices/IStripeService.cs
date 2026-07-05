using Scripters.Regula.Platform.Billing.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.Shared.Application.Model;

namespace Scripters.Regula.Platform.Billing.Application.Internal.OutboundServices;

/// <summary>
///     Puerto hacia el proveedor de pagos. Application solo depende de esta
///     interfaz y de los tipos neutrales que devuelve — la implementación real
///     (que sí importa el SDK de Stripe) vive en Infrastructure.
/// </summary>
public interface IStripeService
{
    /// <summary>
    ///     Crea un Customer + Subscription en Stripe, o recupera una ya existente
    ///     si todavía está activa/incompleta (evita duplicar el cobro si el
    ///     usuario recarga la pantalla de pago).
    /// </summary>
    Task<Result<CheckoutResult>> CreateOrRetrieveSubscriptionAsync(
        string username,
        long userId,
        string? existingCustomerId,
        string? existingSubscriptionId);

    /// <summary>
    ///     Valida la firma Stripe-Signature y traduce el payload a un evento neutral.
    ///     Result.Failure si la firma no es válida (no es de fiar que venga de Stripe).
    /// </summary>
    Result<SubscriptionWebhookEvent> ParseWebhookEvent(string json, string signatureHeader);
}