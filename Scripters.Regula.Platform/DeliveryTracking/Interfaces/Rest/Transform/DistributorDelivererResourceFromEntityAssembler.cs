using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Aggregates;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Entities;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Resources;

namespace Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Transform;

public static class DistributorDelivererResourceFromEntityAssembler
{
    public static DistributorDelivererResource ToResourceFromEntity(
        DeliveryResponsible responsible,
        Delivery? latestDelivery,
        DriverLocation? location)
    {
        var status = DeriveStatus(latestDelivery, location);

        return new DistributorDelivererResource(
            responsible.Id,
            responsible.Name,
            latestDelivery?.Vehicle.Plate ?? string.Empty,
            latestDelivery?.Vehicle.Type ?? string.Empty,
            status,
            location?.Latitude,
            location?.Longitude
        );
    }

    private static string DeriveStatus(Delivery? delivery, DriverLocation? location)
    {
        if (delivery is null)
            return "Sin señal";

        if (delivery.Status == EDeliveryStatus.OnRoute)
        {
            if (location is null || !location.HasActiveSignal())
                return "Sin señal";
            return "En ruta";
        }

        if (delivery.Status == EDeliveryStatus.Delivered)
            return "Completado";

        if (delivery.Status == EDeliveryStatus.NotDelivered)
            return "No entregado";

        return "Pendiente";
    }
}
