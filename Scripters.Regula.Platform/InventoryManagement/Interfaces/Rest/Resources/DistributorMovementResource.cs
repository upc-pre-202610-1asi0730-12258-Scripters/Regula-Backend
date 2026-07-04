namespace Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Resources;

/// <summary>
///     REST representation of a distributor movement entity.
/// </summary>
/// <remarks>Kevin Lopez</remarks>
public record DistributorMovementResource(
    int Id,
    DateTime Timestamp,
    string MovementType,
    string CylinderType,
    int Quantity,
    int UserId,
    string ProviderName,
    string? OutboundType);
