using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Aggregates;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Entities;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Queries;

namespace Scripters.Regula.Platform.InventoryManagement.Application.QueryServices;

public interface IInventoryQueryService
{
    Task<Inventory?> Handle(GetInventoryByIdQuery query, CancellationToken cancellationToken);

    Task<Inventory?> Handle(GetInventoryByUserIdQuery query, CancellationToken cancellationToken);

    Task<IEnumerable<CompanyMovement>> Handle(
        GetCompanyMovementsByInventoryIdQuery query,
        CancellationToken cancellationToken);

    Task<IEnumerable<DistributorMovement>> Handle(
        GetDistributorMovementsByInventoryIdQuery query,
        CancellationToken cancellationToken);

    Task<IEnumerable<GasCylinderStock>> Handle(GetStockByInventoryIdQuery query, CancellationToken cancellationToken);
}
