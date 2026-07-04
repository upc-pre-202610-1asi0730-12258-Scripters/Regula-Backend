using Scripters.Regula.Platform.InventoryManagement.Application.QueryServices;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Aggregates;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Entities;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Queries;
using Scripters.Regula.Platform.InventoryManagement.Domain.Repositories;

namespace Scripters.Regula.Platform.InventoryManagement.Application.Internal.QueryServices;

public class InventoryQueryService(IInventoryRepository inventoryRepository) : IInventoryQueryService
{
    public async Task<Inventory?> Handle(GetInventoryByIdQuery query, CancellationToken cancellationToken)
    {
        return await inventoryRepository.FindSummaryByIdAsync((int)query.InventoryId, cancellationToken);
    }

    public async Task<Inventory?> Handle(GetInventoryByUserIdQuery query, CancellationToken cancellationToken)
    {
        return await inventoryRepository.FindByUserIdAsync(query.UserId, cancellationToken);
    }

    public async Task<IEnumerable<CompanyMovement>> Handle(
        GetCompanyMovementsByInventoryIdQuery query,
        CancellationToken cancellationToken)
    {
        return await inventoryRepository.ListCompanyMovementsByInventoryIdAsync(
            (int)query.InventoryId, query.MovementType, cancellationToken);
    }

    public async Task<IEnumerable<DistributorMovement>> Handle(
        GetDistributorMovementsByInventoryIdQuery query,
        CancellationToken cancellationToken)
    {
        return await inventoryRepository.ListDistributorMovementsByInventoryIdAsync(
            (int)query.InventoryId, query.MovementType, cancellationToken);
    }

    public async Task<IEnumerable<GasCylinderStock>> Handle(
        GetStockByInventoryIdQuery query,
        CancellationToken cancellationToken)
    {
        return await inventoryRepository.ListStockByInventoryIdAsync(
            (int)query.InventoryId, cancellationToken);
    }
}
