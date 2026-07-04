using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Entities;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Events;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.ValueObjects;

namespace Scripters.Regula.Platform.InventoryManagement.Domain.Model.Aggregates;

public partial class Inventory
{
    private const int DefaultLowStockThreshold = 10;

    public MovementRegisteredEvent RegisterCompanyMovement(CompanyMovement movement)
    {
        movement.Quantity.Validate();
        MovementHistory.Add(movement);
        ApplyStockChange(movement);
        return new MovementRegisteredEvent(
            movement.Id, movement.CylinderType, movement.MovementType, movement.Quantity, movement.UserId);
    }

    public MovementRegisteredEvent RegisterDistributorMovement(DistributorMovement movement)
    {
        movement.Quantity.Validate();
        MovementHistory.Add(movement);
        ApplyStockChange(movement);
        return new MovementRegisteredEvent(
            movement.Id, movement.CylinderType, movement.MovementType, movement.Quantity, movement.UserId);
    }

    public StockIncreasedEvent IncreaseStock(ECylinderType cylinderType, Quantity qty)
    {
        var stock = GetOrCreateStock(cylinderType);
        stock.Increase(qty);
        return new StockIncreasedEvent(cylinderType, qty);
    }

    public StockDecreasedEvent DecreaseStock(ECylinderType cylinderType, Quantity qty)
    {
        var stock = GetOrCreateStock(cylinderType);
        stock.Decrease(qty);
        return new StockDecreasedEvent(cylinderType, qty);
    }

    public LowStockDetectedEvent? DetectLowStock(int threshold = DefaultLowStockThreshold)
    {
        var lowStock = StockSummary.StockItems.FirstOrDefault(s => s.IsLow(threshold));
        if (lowStock is null) return null;
        return new LowStockDetectedEvent(lowStock.CylinderType, lowStock.Available);
    }

    private void ApplyStockChange(Movement movement)
    {
        if (movement.IsEntry())
            IncreaseStock(movement.CylinderType, movement.Quantity);
        else if (movement.IsExit())
            DecreaseStock(movement.CylinderType, movement.Quantity);
    }

    private GasCylinderStock GetOrCreateStock(ECylinderType cylinderType)
    {
        var existing = StockSummary.GetByType(cylinderType);
        if (existing is not null) return existing;

        var newStock = new GasCylinderStock(
            cylinderType,
            new Quantity(0), new Quantity(0), new Quantity(0), new Quantity(0));

        StockSummary.Add(newStock);
        return newStock;
    }
}
