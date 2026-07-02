using Microsoft.EntityFrameworkCore;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Aggregates;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Repositories;
using Scripters.Regula.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Scripters.Regula.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace Scripters.Regula.Platform.DeliveryTracking.Infrastructure.Persistence.EFC.Repositories;

public class DeliveryRepository(AppDbContext context)
    : BaseRepository<Delivery>(context), IDeliveryRepository
{
    public async Task<IEnumerable<Delivery>> FindByScheduledDateAndStatusAsync(
        DateOnly date,
        EDeliveryStatus status,
        CancellationToken cancellationToken = default)
    {
        return await context.Set<Delivery>()
            .Where(d => DateOnly.FromDateTime(d.ScheduledTime) == date && d.Status == status)
            .Include(d => d.Vehicle)
            .ToListAsync(cancellationToken);
    }

    public async Task<Delivery?> FindByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Set<Delivery>()
            .Include(d => d.Responsible)
            .Include(d => d.Vehicle)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Delivery>> FindAllWithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await context.Set<Delivery>()
            .Include(d => d.Responsible)
            .Include(d => d.Vehicle)
            .ToListAsync(cancellationToken);
    }
}
