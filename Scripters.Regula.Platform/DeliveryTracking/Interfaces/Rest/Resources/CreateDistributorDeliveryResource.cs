namespace Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Resources;

public record CreateDistributorDeliveryResource(
    int DriverId,
    int ResponsibleId,
    int VehicleId,
    int ItemCount,
    string Cargo,
    string Destination,
    DateTime ScheduledTime
);
