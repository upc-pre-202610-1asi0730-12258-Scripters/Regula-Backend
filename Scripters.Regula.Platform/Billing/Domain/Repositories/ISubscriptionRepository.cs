using Scripters.Regula.Platform.Billing.Domain.Model.Aggregates;
using Scripters.Regula.Platform.Shared.Domain.Repositories;

namespace Scripters.Regula.Platform.Billing.Domain.Repositories;

public interface ISubscriptionRepository : IBaseRepository<Subscription>
{
    Task<Subscription?> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    Task<Subscription?> FindByStripeSubscriptionIdAsync(string stripeSubscriptionId, CancellationToken cancellationToken = default);

    Task<Subscription?> FindByStripeCustomerIdAsync(string stripeCustomerId, CancellationToken cancellationToken = default);
}