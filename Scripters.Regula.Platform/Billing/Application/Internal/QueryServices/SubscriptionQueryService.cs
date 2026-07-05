using Scripters.Regula.Platform.Billing.Application.QueryServices;
using Scripters.Regula.Platform.Billing.Domain.Model.Aggregates;
using Scripters.Regula.Platform.Billing.Domain.Model.Queries;
using Scripters.Regula.Platform.Billing.Domain.Repositories;

namespace Scripters.Regula.Platform.Billing.Application.Internal.QueryServices;

public class SubscriptionQueryService(ISubscriptionRepository subscriptionRepository) : ISubscriptionQueryService
{
    public Task<Subscription?> Handle(GetSubscriptionByUserIdQuery query, CancellationToken cancellationToken)
    {
        return subscriptionRepository.FindByUserIdAsync(query.UserId, cancellationToken);
    }
}