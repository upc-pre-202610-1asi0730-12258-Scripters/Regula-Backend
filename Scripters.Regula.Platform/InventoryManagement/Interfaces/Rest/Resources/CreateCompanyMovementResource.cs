namespace Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Resources;

public record CreateCompanyMovementResource(
    string MovementType,
    string CylinderType,
    int Quantity,
    int UserId,
    string? ProviderName,
    string Destination,
    string MovementReason,
    string Observation);
