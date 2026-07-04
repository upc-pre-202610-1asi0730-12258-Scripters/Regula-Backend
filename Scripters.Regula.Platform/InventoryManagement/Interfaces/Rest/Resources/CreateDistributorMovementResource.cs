namespace Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Resources;

/// <summary>
///     Create distributor movement resource for REST API.
/// </summary>
/// <param name="MovementType">The movement type (Entry, Exit).</param>
/// <param name="CylinderType">The gas cylinder type affected (Kg5, Kg10, Kg15, Kg45).</param>
/// <param name="Quantity">The number of cylinders involved.</param>
/// <param name="UserId">The user ID of the operator.</param>
/// <param name="ProviderName">The supplier name (required for Entry; ignored on Exit).</param>
/// <param name="OutboundType">The outbound type (required for Exit: DirectSale, HomeDelivery, ReturnToSupplier).</param>
public record CreateDistributorMovementResource(
    string MovementType,
    string CylinderType,
    int Quantity,
    int UserId,
    string? ProviderName,
    string? OutboundType);
