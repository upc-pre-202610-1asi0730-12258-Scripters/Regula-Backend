namespace Scripters.Regula.Platform.Billing.Interfaces.Rest.Resources;

/// <summary>Response de GET /api/v1/subscriptions/me.</summary>
public record SubscriptionResource(string Status, DateTime? CurrentPeriodEnd);

/// <summary>Response de POST /api/v1/subscriptions/checkout.</summary>
public record CheckoutResource(string? ClientSecret, bool AlreadyActive);