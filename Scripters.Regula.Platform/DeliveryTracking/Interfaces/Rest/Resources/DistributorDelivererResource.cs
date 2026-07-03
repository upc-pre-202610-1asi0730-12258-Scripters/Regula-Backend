namespace Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Resources;

public record DistributorDelivererResource(
    int Id,
    string Name,
    string VehiclePlate,
    string VehicleType,
    string Status,
    double? Lat,
    double? Lng
);
