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

/// <summary>
/// Controller for managing distributor-specific delivery operations.
/// This controller provides endpoints for retrieving, creating, and updating deliveries from a distributor's perspective.
/// </summary>
/// <remarks>
/// The route uses the bounded context prefix: api/v1/{bounded-context}/{resource}.
/// "distributor-deliverers" no longer lives here (it previously escaped with an absolute route, [HttpGet("/api/v1/distributor-deliverers")])
/// — it is a distinct resource and now has its own controller: <see cref="DistributorDeliverersController"/>.
/// </remarks>
/// <param name="deliveryQueryService">The delivery query service for retrieving delivery information.</param>
/// <param name="deliveryCommandService">The delivery command service for handling delivery-related commands.</param>
/// <param name="driverLocationRepository">The repository for driver location data.</param>
[ApiController]
[Route("api/v1/delivery-tracking/distributor-deliveries")]
[Produces("application/json")]
public class DistributorDeliveriesController(
    IDeliveryQueryService deliveryQueryService,
    IDeliveryCommandService deliveryCommandService,
    IDriverLocationRepository driverLocationRepository) : ControllerBase
{
    /// <summary>
    /// Retrieves all deliveries with responsible, vehicle, and location details for the distributor frontend.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="DistributorDeliveryResource"/> objects.</returns>
    /// <response code="200">Returns the list of deliveries.</response>
    [HttpGet]
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

    /// <summary>
    /// Creates a new delivery with a default status of Pending.
    /// </summary>
    /// <param name="resource">The resource containing details for the new delivery.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="DistributorDeliveryResource"/> representing the newly created delivery.</returns>
    /// <response code="201">Returns the newly created delivery.</response>
    /// <response code="400">If the input is invalid.</response>
    [HttpPost]
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

        return Created($"/api/v1/delivery-tracking/distributor-deliveries/{delivery.Value!.Id}", deliveryResource);
    }

    /// <summary>
    /// Updates the status of an existing delivery.
    /// </summary>
    /// <remarks>
    /// Valid status transitions are: Pending → OnRoute, OnRoute → Delivered, OnRoute → NotDelivered.
    /// </remarks>
    /// <param name="id">The unique identifier of the delivery to update.</param>
    /// <param name="resource">The resource containing the new status for the delivery.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>An <see cref="IActionResult"/> containing the updated delivery details.</returns>
    /// <response code="200">Returns the updated delivery details.</response>
    /// <response code="400">If the provided status value is invalid.</response>
    /// <response code="404">If a delivery with the specified ID is not found.</response>
    /// <response code="422">If the requested status transition is invalid.</response>
    [HttpPatch("{id:int}/status")]
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
}