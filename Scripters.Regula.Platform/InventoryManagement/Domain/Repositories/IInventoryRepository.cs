namespace Scripters.Regula.Platform.InventoryManagement.Domain.Repositories;

using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Aggregates;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Entities;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.Shared.Domain.Repositories;

public interface IInventoryRepository : IBaseRepository<Inventory>
{
    Task<Inventory?> FindSummaryByIdAsync(int id, CancellationToken cancellationToken);

    Task<IEnumerable<CompanyMovement>> ListCompanyMovementsByInventoryIdAsync(
        int inventoryId,
        EMovementType? movementType,
        CancellationToken cancellationToken);

    Task<IEnumerable<DistributorMovement>> ListDistributorMovementsByInventoryIdAsync(
        int inventoryId,
        EMovementType? movementType,
        CancellationToken cancellationToken);

    Task<IEnumerable<GasCylinderStock>> ListStockByInventoryIdAsync(
        int inventoryId,
        CancellationToken cancellationToken);

    Task<Inventory?> FindByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken);
}
