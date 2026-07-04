using Scripters.Regula.Platform.InventoryManagement.Domain.Model.ValueObjects;

namespace Scripters.Regula.Platform.InventoryManagement.Domain.Model.Entities;

public abstract partial class Movement
{
    public const string OwnerProviderName = "Propietario";

    protected Movement()
    {
        Timestamp    = DateTime.UtcNow;
        Quantity     = new Quantity(1);
        UserId    = new UserId(0);
        ProviderName = new ProviderName(string.Empty);
    }

    protected Movement(
        EMovementType movementType,
        ECylinderType cylinderType,
        Quantity      quantity,
        ProviderName  providerName,
        UserId     userId)
    {
        Timestamp    = DateTime.UtcNow;
        MovementType = movementType;
        CylinderType = cylinderType;
        Quantity     = quantity;
        ProviderName = providerName;
        UserId    = userId;
    }

    public int Id { get; private set; }

    public DateTime      Timestamp    { get; private set; }
    public EMovementType MovementType { get; private set; }
    public ECylinderType CylinderType { get; private set; }
    public Quantity      Quantity     { get; private set; }
    public ProviderName  ProviderName  { get; private set; }
    public UserId UserId { get; private set; }

    public bool IsEntry() => MovementType == EMovementType.Entry;

    public bool IsExit() => MovementType == EMovementType.Exit;
}
