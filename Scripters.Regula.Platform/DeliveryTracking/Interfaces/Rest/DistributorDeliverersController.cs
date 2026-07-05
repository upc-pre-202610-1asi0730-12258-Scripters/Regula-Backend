using Microsoft.AspNetCore.Mvc;
using Scripters.Regula.Platform.DeliveryTracking.Application.QueryServices;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Queries;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Repositories;
using Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Resources;
using Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest;

/// <summary>
/// Controller for managing distributor deliverers.
/// </summary>
/// <param name="deliveryQueryService">The delivery query service for retrieving delivery information.</param>
/// <param name="driverLocationRepository">The repository for driver location data.</param>
[ApiController]
[Route("api/v1/delivery-tracking/distributor-deliverers")]
[Produces("application/json")]
public class DistributorDeliverersController(
    IDeliveryQueryService deliveryQueryService,
    IDriverLocationRepository driverLocationRepository) : ControllerBase
{
    /// <summary>
    /// Retrieves all delivery responsibles along with their current vehicle and GPS location for the distributor frontend.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="DistributorDelivererResource"/> objects.</returns>
    /// <response code="200">Returns the list of deliverers.</response>
    [HttpGet]
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