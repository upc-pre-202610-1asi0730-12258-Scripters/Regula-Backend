using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Commands;
using Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Resources;

namespace Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Transform;

public static class CreateDistributorDeliveryCommandFromResourceAssembler
{
    public static CreateDistributorDeliveryCommand ToCommandFromResource(CreateDistributorDeliveryResource resource)
    {
        return new CreateDistributorDeliveryCommand(
            resource.DriverId,
            resource.ResponsibleId,
            resource.VehicleId,
            resource.ItemCount,
            resource.Cargo,
            resource.Destination,
            resource.ScheduledTime
        );
    }
}
