namespace Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Resources;

public record DeliveryResponsibleCatalogItem(int Id, string Name);

public record DeliveryVehicleCatalogItem(int Id, string Plate, string Type, string Brand);