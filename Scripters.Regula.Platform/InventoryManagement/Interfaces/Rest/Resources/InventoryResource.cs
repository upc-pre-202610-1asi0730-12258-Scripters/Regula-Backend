namespace Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Resources;

/// <summary>
///     REST representation of an inventory aggregate.
/// </summary>
/// <remarks>Kevin Lopez</remarks>
public record InventoryResource(
    int Id,
    long OwnerProfileId,
    string InventoryType,
    int TotalAvailable);
