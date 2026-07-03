namespace Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Commands;

public record CreateDistributorDeliveryCommand(
    int DriverId,
    int ResponsibleId,
    int VehicleId,
    int ItemCount,
    string Cargo,
    string Destination,
    DateTime ScheduledTime
);
