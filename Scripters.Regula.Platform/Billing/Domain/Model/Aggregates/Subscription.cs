using Scripters.Regula.Platform.Billing.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.Shared.Domain.Model.Entities;

namespace Scripters.Regula.Platform.Billing.Domain.Model.Aggregates;

/// <summary>
///     Local record of a user's Stripe subscription. Stripe remains the
///     source of truth for billing itself; this table only mirrors enough
///     state (status, period end) to answer "can this user in?" quickly,
///     without calling Stripe on every request.
/// </summary>
public class Subscription : IAuditableEntity
{
    public int Id { get; private set; }
    public long UserId { get; private set; }
    public string StripeCustomerId { get; private set; } = null!;
    public string? StripeSubscriptionId { get; private set; }
    public ESubscriptionStatus Status { get; private set; } = ESubscriptionStatus.None;
    public DateTime? CurrentPeriodEnd { get; private set; }

    // IAuditableEntity: el AuditableEntityInterceptor los llena solo en cada
    // SaveChanges (Added -> CreatedAt+UpdatedAt, Modified -> UpdatedAt). Antes
    // esta clase tenía DateTime? sueltos, sin implementar la interfaz, así que
    // el interceptor nunca los tocaba y quedaban siempre en null.
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected Subscription()
    {
    }

    public Subscription(long userId, string stripeCustomerId)
    {
        UserId = userId;
        StripeCustomerId = stripeCustomerId;
        Status = ESubscriptionStatus.None;
    }

    public bool IsActive => Status == ESubscriptionStatus.Active;

    public void AttachStripeSubscription(string stripeSubscriptionId, ESubscriptionStatus status, DateTime? currentPeriodEnd)
    {
        StripeSubscriptionId = stripeSubscriptionId;
        Status = status;
        CurrentPeriodEnd = currentPeriodEnd;
    }

    public void UpdateStatus(ESubscriptionStatus status, DateTime? currentPeriodEnd)
    {
        Status = status;
        CurrentPeriodEnd = currentPeriodEnd;
    }
}