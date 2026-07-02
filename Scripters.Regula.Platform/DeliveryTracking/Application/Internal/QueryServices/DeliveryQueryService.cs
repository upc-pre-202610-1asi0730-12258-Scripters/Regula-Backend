using Scripters.Regula.Platform.DeliveryTracking.Application.QueryServices;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Errors;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Aggregates;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Queries;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Repositories;
using Scripters.Regula.Platform.Shared.Application.Model;

namespace Scripters.Regula.Platform.DeliveryTracking.Application.Internal.QueryServices;

public class DeliveryQueryService(
    IDeliveryRepository deliveryRepository) : IDeliveryQueryService
{
    public async Task<Result<IEnumerable<Delivery>>> Handle(
        GetDeliveriesByDateAndStatusQuery query,
        CancellationToken cancellationToken = default)
    {
        var deliveries = await deliveryRepository.FindByScheduledDateAndStatusAsync(
            query.Date, query.Status, cancellationToken);

        return Result<IEnumerable<Delivery>>.Success(deliveries);
    }

    public async Task<Result<Delivery>> Handle(
        GetDeliveryByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var delivery = await deliveryRepository.FindByIdWithDetailsAsync(query.DeliveryId, cancellationToken);

        if (delivery is null)
            return Result<Delivery>.Failure(
                DeliveryTrackingErrors.DeliveryNotFound,
                $"Delivery with id {query.DeliveryId} was not found.");

        return Result<Delivery>.Success(delivery);
    }

    public async Task<Result<IEnumerable<Delivery>>> Handle(
        GetAllDistributorDeliveriesQuery query,
        CancellationToken cancellationToken = default)
    {
        var deliveries = await deliveryRepository.FindAllWithDetailsAsync(cancellationToken);
        return Result<IEnumerable<Delivery>>.Success(deliveries);
    }
}
