using Microsoft.AspNetCore.Mvc;
using Scripters.Regula.Platform.DeliveryTracking.Application.QueryServices;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Queries;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Repositories;
using Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Resources;
using Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest;

[ApiController]
[Produces("application/json")]
public class DistributorDeliveriesController(
    IDeliveryQueryService deliveryQueryService,
    IDriverLocationRepository driverLocationRepository) : ControllerBase
{
    [HttpGet("distributorDeliveries")]
    [SwaggerOperation(
        Summary = "Get all distributor deliveries",
        Description = "Returns all deliveries with responsible, vehicle and location details for the distributor frontend.",
        OperationId = "GetDistributorDeliveries")]
    [SwaggerResponse(StatusCodes.Status200OK, "Deliveries returned", typeof(IEnumerable<DistributorDeliveryResource>))]
    public async Task<IActionResult> GetDistributorDeliveries(CancellationToken cancellationToken)
    {
        var result = await deliveryQueryService.Handle(new GetAllDistributorDeliveriesQuery(), cancellationToken);

        var deliveries = result.Value!.ToList();
        var deliveryIds = deliveries.Select(d => d.Id);
        var locations = (await driverLocationRepository.FindByDeliveryIdsAsync(deliveryIds, cancellationToken))
            .ToDictionary(l => l.DeliveryId);

        var resources = deliveries.Select(d =>
            DistributorDeliveryResourceFromEntityAssembler.ToResourceFromEntity(
                d, locations.GetValueOrDefault(d.Id)));

        return Ok(resources);
    }

    [HttpGet("distributorDeliverers")]
    [SwaggerOperation(
        Summary = "Get all distributor deliverers",
        Description = "Returns all delivery responsibles with their current vehicle and GPS location for the distributor frontend.",
        OperationId = "GetDistributorDeliverers")]
    [SwaggerResponse(StatusCodes.Status200OK, "Deliverers returned", typeof(IEnumerable<DistributorDelivererResource>))]
    public async Task<IActionResult> GetDistributorDeliverers(CancellationToken cancellationToken)
    {
        var result = await deliveryQueryService.Handle(new GetAllDistributorDeliveriesQuery(), cancellationToken);

        var deliveries = result.Value!.ToList();
        var deliveryIds = deliveries.Select(d => d.Id);
        var locations = (await driverLocationRepository.FindByDeliveryIdsAsync(deliveryIds, cancellationToken))
            .ToDictionary(l => l.DeliveryId);

        var delivererGroups = deliveries
            .GroupBy(d => d.ResponsibleId)
            .Select(g =>
            {
                var latestDelivery = g.OrderByDescending(d => d.ScheduledTime).First();
                var location = locations.GetValueOrDefault(latestDelivery.Id);
                return DistributorDelivererResourceFromEntityAssembler.ToResourceFromEntity(
                    latestDelivery.Responsible, latestDelivery, location);
            });

        return Ok(delivererGroups);
    }
}
