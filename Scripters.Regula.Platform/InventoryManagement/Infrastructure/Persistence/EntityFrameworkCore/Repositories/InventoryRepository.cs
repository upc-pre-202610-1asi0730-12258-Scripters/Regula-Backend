using Microsoft.EntityFrameworkCore;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Aggregates;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Entities;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.InventoryManagement.Domain.Repositories;
using Scripters.Regula.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Scripters.Regula.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace Scripters.Regula.Platform.InventoryManagement.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class InventoryRepository(AppDbContext context)
    : BaseRepository<Inventory>(context), IInventoryRepository
{
    public async Task<Inventory?> FindByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken)
    {
        return await Context.Set<Inventory>()
            .Include(i => i.StockItems)
            .FirstOrDefaultAsync(i => i.UserId == userId, cancellationToken);
    }

    public async Task<Inventory?> FindSummaryByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await Context.Set<Inventory>()
            .Include(i => i.StockItems)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<CompanyMovement>> ListCompanyMovementsByInventoryIdAsync(
        int inventoryId,
        EMovementType? movementType,
        CancellationToken cancellationToken)
    {
        var query = Context.Set<CompanyMovement>()
            .Where(m => EF.Property<int>(m, "inventory_id") == inventoryId);

        if (movementType.HasValue)
            query = query.Where(m => m.MovementType == movementType.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DistributorMovement>> ListDistributorMovementsByInventoryIdAsync(
        int inventoryId,
        EMovementType? movementType,
        CancellationToken cancellationToken)
    {
        var query = Context.Set<DistributorMovement>()
            .Where(m => EF.Property<int>(m, "inventory_id") == inventoryId);

        if (movementType.HasValue)
            query = query.Where(m => m.MovementType == movementType.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<GasCylinderStock>> ListStockByInventoryIdAsync(
        int inventoryId,
        CancellationToken cancellationToken)
    {
        return await Context.Set<GasCylinderStock>()
            .Where(s => EF.Property<int>(s, "inventory_id") == inventoryId)
            .ToListAsync(cancellationToken);
    }

    public new async Task<Inventory?> FindByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await Context.Set<Inventory>()
            .Include(i => i.StockItems)
            .Include(i => i.Movements)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }
}
