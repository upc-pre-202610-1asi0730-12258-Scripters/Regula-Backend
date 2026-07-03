using Microsoft.AspNetCore.Mvc;
using Scripters.Regula.Platform.DeliveryTracking.Application.CommandServices;
using Scripters.Regula.Platform.DeliveryTracking.Application.QueryServices;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Errors;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Queries;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Repositories;
using Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Resources;
using Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest;

[ApiController]
[Produces("application/json")]
public class DistributorDeliveriesController(
    IDeliveryQueryService deliveryQueryService,
    IDeliveryCommandService deliveryCommandService,
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

    [HttpPost("distributorDeliveries")]
    [SwaggerOperation(
        Summary = "Create a distributor delivery",
        Description = "Creates a new delivery with status Pending.",
        OperationId = "CreateDistributorDelivery")]
    [SwaggerResponse(StatusCodes.Status201Created, "Delivery created", typeof(DistributorDeliveryResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input")]
    public async Task<IActionResult> CreateDistributorDelivery(
        [FromBody] CreateDistributorDeliveryResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateDistributorDeliveryCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await deliveryCommandService.Handle(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Message });

        var delivery = await deliveryQueryService.Handle(
            new GetDeliveryByIdQuery(result.Value!.Id), cancellationToken);

        var deliveryResource = DistributorDeliveryResourceFromEntityAssembler
            .ToResourceFromEntity(delivery.Value!, null);

        return Created($"/distributorDeliveries/{delivery.Value!.Id}", deliveryResource);
    }

    [HttpPatch("distributorDeliveries/{id:int}/status")]
    [SwaggerOperation(
        Summary = "Update delivery status",
        Description = "Updates the status of a delivery. Valid transitions: Pending→OnRoute, OnRoute→Delivered, OnRoute→NotDelivered.",
        OperationId = "UpdateDistributorDeliveryStatus")]
    [SwaggerResponse(StatusCodes.Status200OK, "Status updated", typeof(DistributorDeliveryResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Delivery not found")]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Invalid status transition")]
    public async Task<IActionResult> UpdateDistributorDeliveryStatus(
        [FromRoute] int id,
        [FromBody] UpdateDeliveryStatusResource resource,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<EDeliveryStatus>(resource.Status, ignoreCase: true, out var status))
            return BadRequest(new { error = $"Invalid status '{resource.Status}'. Allowed: Pending, OnRoute, Delivered, NotDelivered." });

        var command = UpdateDeliveryStatusCommandFromResourceAssembler.ToCommandFromResource(id, resource, status);
        var result = await deliveryCommandService.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error switch
            {
                DeliveryTrackingErrors.DeliveryNotFound => NotFound(new { error = result.Message }),
                DeliveryTrackingErrors.InvalidStatusTransition => UnprocessableEntity(new { error = result.Message }),
                _ => BadRequest(new { error = result.Message })
            };
        }

        var delivery = await deliveryQueryService.Handle(
            new GetDeliveryByIdQuery(id), cancellationToken);

        var location = await driverLocationRepository.FindByDeliveryIdAsync(id, cancellationToken);

        var deliveryResource = DistributorDeliveryResourceFromEntityAssembler
            .ToResourceFromEntity(delivery.Value!, location);

        return Ok(deliveryResource);
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
