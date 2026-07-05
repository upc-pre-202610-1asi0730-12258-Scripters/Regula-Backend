namespace Scripters.Regula.Platform.Billing.Domain.Model.ValueObjects;

/// <summary>
///     Local mirror of Stripe's subscription status, simplified to what this
///     app actually needs to decide access (see Subscription.IsActive).
/// </summary>
public enum ESubscriptionStatus
{
    None,
    Incomplete,
    Active,
    PastDue,
    Canceled
}