using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Entities;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.ValueObjects;

namespace Scripters.Regula.Platform.InventoryManagement.Domain.Model.Aggregates;

public partial class Inventory
{
    public Inventory()
    {
        UserId          = new UserId(0);
        InventoryType   = EInventoryType.Company;
        StockSummary    = new StockSummary();
        MovementHistory = new MovementHistory();
    }

    public Inventory(UserId userId, EInventoryType inventoryType)
    {
        UserId          = userId;
        InventoryType   = inventoryType;
        StockSummary    = new StockSummary();
        MovementHistory = new MovementHistory();
    }

    public int Id { get; private set; }

    public UserId UserId { get; private set; }

    public EInventoryType InventoryType { get; private set; }

    public StockSummary StockSummary { get; private set; }

    public MovementHistory MovementHistory { get; private set; }

    public ICollection<GasCylinderStock> StockItems => StockSummary.StockItems;

    public ICollection<Movement> Movements => MovementHistory.Movements;

    public bool IsCompany() => InventoryType == EInventoryType.Company;
}
