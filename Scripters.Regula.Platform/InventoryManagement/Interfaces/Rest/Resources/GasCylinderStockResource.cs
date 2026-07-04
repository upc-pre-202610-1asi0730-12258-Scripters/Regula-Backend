namespace Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Resources;

/// <summary>
///     REST representation of a gas cylinder stock entity.
/// </summary>
/// <remarks>Kevin Lopez</remarks>
public record GasCylinderStockResource(
    int Id,
    string CylinderType,
    int Available,
    int InTransit,
    int Observed,
    int OutOfService);
