using Scripters.Regula.Platform.Billing.Domain.Model.Aggregates;
using Scripters.Regula.Platform.Billing.Domain.Model.Queries;

namespace Scripters.Regula.Platform.Billing.Application.QueryServices;

public interface ISubscriptionQueryService
{
    Task<Subscription?> Handle(GetSubscriptionByUserIdQuery query, CancellationToken cancellationToken);
}