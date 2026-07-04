using Scripters.Regula.Platform.InventoryManagement.Domain.Model.ValueObjects;

namespace Scripters.Regula.Platform.InventoryManagement.Domain.Model.Commands;

public record CreateDistributorMovementCommand(
    long           InventoryId,
    EMovementType  MovementType,
    ECylinderType  CylinderType,
    int            Quantity,
    int            UserId,
    string?        ProviderName,
    EOutboundType? OutboundType);
