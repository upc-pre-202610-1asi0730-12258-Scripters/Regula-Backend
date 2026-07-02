using Microsoft.EntityFrameworkCore;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Entities;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Repositories;
using Scripters.Regula.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Scripters.Regula.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace Scripters.Regula.Platform.DeliveryTracking.Infrastructure.Persistence.EFC.Repositories;

public class DriverLocationRepository(AppDbContext context)
    : BaseRepository<DriverLocation>(context), IDriverLocationRepository
{
    public async Task<DriverLocation?> FindByDeliveryIdAsync(int deliveryId, CancellationToken cancellationToken = default)
    {
        return await context.Set<DriverLocation>()
            .FirstOrDefaultAsync(l => l.DeliveryId == deliveryId, cancellationToken);
    }

    public async Task<IEnumerable<DriverLocation>> FindByDeliveryIdsAsync(IEnumerable<int> deliveryIds, CancellationToken cancellationToken = default)
    {
        var ids = deliveryIds.ToList();
        return await context.Set<DriverLocation>()
            .Where(l => ids.Contains(l.DeliveryId))
            .ToListAsync(cancellationToken);
    }
}
