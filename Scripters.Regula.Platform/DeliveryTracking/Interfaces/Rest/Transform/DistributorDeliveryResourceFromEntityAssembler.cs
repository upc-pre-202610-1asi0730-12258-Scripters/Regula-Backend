using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Aggregates;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Entities;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Resources;

namespace Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Transform;

public static class DistributorDeliveryResourceFromEntityAssembler
{
    public static DistributorDeliveryResource ToResourceFromEntity(Delivery entity, DriverLocation? location)
    {
        return new DistributorDeliveryResource(
            entity.Id,
            entity.ScheduledTime.ToString("dd/MM/yyyy"),
            entity.ScheduledTime.ToString("HH:mm"),
            entity.Responsible.Name,
            entity.ResponsibleId,
            entity.Vehicle.Type,
            entity.Vehicle.Plate,
            entity.Cargo,
            entity.Destination,
            MapStatus(entity.Status),
            location?.Eta?.ToString("HH:mm"),
            entity.DeliveredAt
        );
    }

    private static string MapStatus(EDeliveryStatus status)
    {
        return status switch
        {
            EDeliveryStatus.Pending => "Pendiente",
            EDeliveryStatus.OnRoute => "En Ruta",
            EDeliveryStatus.Delivered => "Completado",
            EDeliveryStatus.NotDelivered => "No entregado",
            _ => status.ToString()
        };
    }
}
