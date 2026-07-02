using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Aggregates;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Queries;
using Scripters.Regula.Platform.Shared.Application.Model;

namespace Scripters.Regula.Platform.DeliveryTracking.Application.QueryServices;

public interface IDeliveryQueryService
{
    Task<Result<IEnumerable<Delivery>>> Handle(GetDeliveriesByDateAndStatusQuery query, CancellationToken cancellationToken = default);

    Task<Result<Delivery>> Handle(GetDeliveryByIdQuery query, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<Delivery>>> Handle(GetAllDistributorDeliveriesQuery query, CancellationToken cancellationToken = default);
}
