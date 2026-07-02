using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Aggregates;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.Shared.Domain.Repositories;

namespace Scripters.Regula.Platform.DeliveryTracking.Domain.Repositories;

public interface IDeliveryRepository : IBaseRepository<Delivery>
{
    Task<IEnumerable<Delivery>> FindByScheduledDateAndStatusAsync(DateOnly date, EDeliveryStatus status, CancellationToken cancellationToken = default);

    Task<Delivery?> FindByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Delivery>> FindAllWithDetailsAsync(CancellationToken cancellationToken = default);
}
