using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Entities;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Repositories;
using Scripters.Regula.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Scripters.Regula.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace Scripters.Regula.Platform.DeliveryTracking.Infrastructure.Persistence.EFC.Repositories;

public class DeliveryResponsibleRepository(AppDbContext context)
    : BaseRepository<DeliveryResponsible>(context), IDeliveryResponsibleRepository
{
}