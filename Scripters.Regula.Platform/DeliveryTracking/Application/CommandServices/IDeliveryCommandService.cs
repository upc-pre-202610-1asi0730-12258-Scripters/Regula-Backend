using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Aggregates;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Commands;
using Scripters.Regula.Platform.Shared.Application.Model;

namespace Scripters.Regula.Platform.DeliveryTracking.Application.CommandServices;

public interface IDeliveryCommandService
{
    Task<Result<Delivery>> Handle(UpdateDeliveryStatusCommand command, CancellationToken cancellationToken = default);

    Task<Result<Delivery>> Handle(CreateDistributorDeliveryCommand command, CancellationToken cancellationToken = default);
}
