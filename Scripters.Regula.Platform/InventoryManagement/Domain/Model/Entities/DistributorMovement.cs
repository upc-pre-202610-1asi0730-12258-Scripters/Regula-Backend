using Scripters.Regula.Platform.InventoryManagement.Domain.Model.ValueObjects;

namespace Scripters.Regula.Platform.InventoryManagement.Domain.Model.Entities;

public class DistributorMovement : Movement
{
    public DistributorMovement()
    {
    }

    public DistributorMovement(
        EMovementType  movementType,
        ECylinderType  cylinderType,
        Quantity       quantity,
        ProviderName   providerName,
        EOutboundType? outboundType,
        UserId      userId)
        : base(movementType, cylinderType, quantity, providerName, userId)
    {
        OutboundType = outboundType;
    }

    public EOutboundType? OutboundType { get; private set; }
}
