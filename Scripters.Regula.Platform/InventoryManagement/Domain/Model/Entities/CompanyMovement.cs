using Scripters.Regula.Platform.InventoryManagement.Domain.Model.ValueObjects;

namespace Scripters.Regula.Platform.InventoryManagement.Domain.Model.Entities;

public class CompanyMovement : Movement
{
    public CompanyMovement()
    {
        Destination    = new Destination(string.Empty);
        MovementReason = new MovementReason(string.Empty);
        Observation    = new Observation(string.Empty);
    }

    public CompanyMovement(
        EMovementType  movementType,
        ECylinderType  cylinderType,
        Quantity       quantity,
        ProviderName   providerName,
        Destination    destination,
        MovementReason movementReason,
        Observation    observation,
        UserId      userId)
        : base(movementType, cylinderType, quantity, providerName, userId)
    {
        Destination    = destination;
        MovementReason = movementReason;
        Observation    = observation;
    }

    public Destination    Destination    { get; private set; }
    public MovementReason MovementReason { get; private set; }
    public Observation    Observation    { get; private set; }
}
