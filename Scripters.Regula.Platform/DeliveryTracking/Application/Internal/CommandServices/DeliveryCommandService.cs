using Scripters.Regula.Platform.DeliveryTracking.Application.CommandServices;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Errors;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Aggregates;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Commands;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Repositories;
using Scripters.Regula.Platform.Shared.Application.Model;
using Scripters.Regula.Platform.Shared.Domain.Repositories;

namespace Scripters.Regula.Platform.DeliveryTracking.Application.Internal.CommandServices;

public class DeliveryCommandService(
    IDeliveryRepository deliveryRepository,
    IUnitOfWork unitOfWork) : IDeliveryCommandService
{
    public async Task<Result<Delivery>> Handle(
        UpdateDeliveryStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var delivery = await deliveryRepository.FindByIdAsync(command.DeliveryId, cancellationToken);

        if (delivery is null)
            return Result<Delivery>.Failure(
                DeliveryTrackingErrors.DeliveryNotFound,
                $"Delivery with id {command.DeliveryId} was not found.");

        if (!delivery.CanTransitionTo(command.Status))
            return Result<Delivery>.Failure(
                DeliveryTrackingErrors.InvalidStatusTransition,
                $"Transición de estado no permitida: {delivery.Status.ToString().ToUpperInvariant()} → {command.Status.ToString().ToUpperInvariant()}");

        delivery.UpdateStatus(command.Status, command.DeliveredAt);

        deliveryRepository.Update(delivery);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result<Delivery>.Success(delivery);
    }

    public async Task<Result<Delivery>> Handle(
        CreateDistributorDeliveryCommand command,
        CancellationToken cancellationToken = default)
    {
        var delivery = new Delivery(
            command.DriverId,
            command.ResponsibleId,
            command.VehicleId,
            command.ItemCount,
            command.Cargo,
            command.Destination,
            command.ScheduledTime);

        await deliveryRepository.AddAsync(delivery, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result<Delivery>.Success(delivery);
    }
}
