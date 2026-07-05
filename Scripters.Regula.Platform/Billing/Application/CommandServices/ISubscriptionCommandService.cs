using Scripters.Regula.Platform.Billing.Domain.Model.Commands;
using Scripters.Regula.Platform.Shared.Application.Model;

namespace Scripters.Regula.Platform.Billing.Application.CommandServices;

public interface ISubscriptionCommandService
{
    /// <summary>
    ///     Result.Value es el clientSecret para confirmar el pago en el frontend,
    ///     o null si el usuario ya tiene una suscripción activa (nada que cobrar).
    ///     Result.IsFailure solo si Stripe mismo falló (ver BillingErrors).
    /// </summary>
    Task<Result<string?>> Handle(CreateCheckoutCommand command, CancellationToken cancellationToken);

    /// <summary>Procesa un evento de webhook ya autenticado por su firma.</summary>
    Task<Result> HandleWebhook(string json, string signatureHeader, CancellationToken cancellationToken);
}