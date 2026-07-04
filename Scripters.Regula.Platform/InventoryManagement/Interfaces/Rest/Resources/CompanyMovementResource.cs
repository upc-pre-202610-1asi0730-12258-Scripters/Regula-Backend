namespace Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Resources;

/// <summary>
///     REST representation of a company movement entity.
/// </summary>
/// <remarks>Kevin Lopez</remarks>
public record CompanyMovementResource(
    int Id,
    DateTime Timestamp,
    string MovementType,
    string CylinderType,
    int Quantity,
    int UserId,
    string ProviderName,
    string Destination,
    string MovementReason,
    string Observation);
