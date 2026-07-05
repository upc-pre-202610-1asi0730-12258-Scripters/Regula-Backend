namespace Scripters.Regula.Platform.Billing.Domain.Model.ValueObjects;

public enum EWebhookEventType
{
    SubscriptionUpserted,
    SubscriptionCanceled,
    Unhandled
}

/// <summary>
///     Traducción neutral de un evento de webhook de Stripe. El command service
///     (Application) solo conoce esto — nunca Stripe.Event ni Stripe.Subscription.
/// </summary>
public record SubscriptionWebhookEvent(
    EWebhookEventType EventType,
    string? StripeSubscriptionId,
    string? StripeCustomerId,
    ESubscriptionStatus Status,
    DateTime? CurrentPeriodEnd);