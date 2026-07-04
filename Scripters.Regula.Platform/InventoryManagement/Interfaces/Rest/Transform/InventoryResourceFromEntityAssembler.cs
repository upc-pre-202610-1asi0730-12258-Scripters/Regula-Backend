using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Aggregates;
using Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Resources;

namespace Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Transform;

public static class InventoryResourceFromEntityAssembler
{
    public static InventoryResource ToResourceFromEntity(Inventory entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity),
                "Inventory entity cannot be null when converting to resource.");

        return new InventoryResource(
            entity.Id,
            entity.UserId.Value,
            entity.InventoryType.ToString(),
            entity.StockSummary.TotalAvailable());
    }
}
