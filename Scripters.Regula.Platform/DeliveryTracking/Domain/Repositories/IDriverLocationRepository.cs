using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Entities;
using Scripters.Regula.Platform.Shared.Domain.Repositories;

namespace Scripters.Regula.Platform.DeliveryTracking.Domain.Repositories;

public interface IDriverLocationRepository : IBaseRepository<DriverLocation>
{
    Task<DriverLocation?> FindByDeliveryIdAsync(int deliveryId, CancellationToken cancellationToken = default);

    Task<IEnumerable<DriverLocation>> FindByDeliveryIdsAsync(IEnumerable<int> deliveryIds, CancellationToken cancellationToken = default);
}