namespace Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Resources;

/// <summary>
///     REST representation of a company movement entity.
/// </summary>
/// <remarks>Kevin Lopez</remarks>
public record InventoryCompanyMovementItem(
    int Id,
    DateTime Timestamp,
    string MovementType,
    string CylinderType,
    int Quantity,
    long ProfileId,
    string ProviderName,
    string Destination,
    string MovementReason,
    string Observation);

/// <summary>
///     REST representation of a distributor movement entity.
/// </summary>
/// <remarks>Kevin Lopez</remarks>
public record InventoryDistributorMovementItem(
    int Id,
    DateTime Timestamp,
    string MovementType,
    string CylinderType,
    int Quantity,
    long ProfileId,
    string ProviderName,
    string? OutboundType);
