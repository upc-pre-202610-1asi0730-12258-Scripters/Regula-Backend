using Scripters.Regula.Platform.InventoryManagement.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.Shared.Domain.Model.Events;

namespace Scripters.Regula.Platform.InventoryManagement.Domain.Model.Events;

public class MovementRegisteredEvent(
    int           movementId,
    ECylinderType cylinderType,
    EMovementType movementType,
    Quantity      quantity,
    UserId        userId) : IEvent
{
    public int           MovementId   { get; } = movementId;
    public ECylinderType CylinderType { get; } = cylinderType;
    public EMovementType MovementType { get; } = movementType;
    public Quantity      Quantity     { get; } = quantity;
    public UserId        UserId       { get; } = userId;
}
