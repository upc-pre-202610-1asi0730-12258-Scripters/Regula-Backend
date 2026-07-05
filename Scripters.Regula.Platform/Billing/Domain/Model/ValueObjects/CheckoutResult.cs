namespace Scripters.Regula.Platform.Billing.Domain.Model.ValueObjects;

/// <summary>
///     Resultado de crear o recuperar una suscripción, ya traducido al vocabulario
///     propio de la aplicación. Nada de aquí conoce tipos del SDK de Stripe —
///     ese es exactamente el trabajo de la capa anticorrupción (StripeService,
///     en Infrastructure).
/// </summary>
public record CheckoutResult(string CustomerId, string SubscriptionId, string? ClientSecret, ESubscriptionStatus Status);