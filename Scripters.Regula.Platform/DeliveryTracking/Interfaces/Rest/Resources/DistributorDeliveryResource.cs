namespace Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Resources;

public record DistributorDeliveryResource(
    int Id,
    string Date,
    string Time,
    string DelivererName,
    int DelivererId,
    string VehicleType,
    string VehiclePlate,
    string Cargo,
    string Destination,
    string Status,
    string? Eta,
    string? RealTime
);
