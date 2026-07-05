using Microsoft.AspNetCore.Mvc;
using Scripters.Regula.Platform.DeliveryTracking.Application.CommandServices;
using Scripters.Regula.Platform.DeliveryTracking.Application.QueryServices;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Errors;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Queries;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Resources;
using Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest;

[ApiController]
[Route("api/v1/delivery-tracking/deliveries")]
[Produces("application/json")]
public class DeliveriesController(
    IDeliveryQueryService deliveryQueryService,
    IDeliveryCommandService deliveryCommandService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get deliveries by date and status",
        Description = "Returns the list of deliveries for a given date filtered by status (PENDING, ON_ROUTE or DELIVERED).",
        OperationId = "GetDeliveries")]
    [SwaggerResponse(StatusCodes.Status200OK, "Deliveries returned", typeof(IEnumerable<DeliveryResource>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid status value")]
    public async Task<IActionResult> GetDeliveries(
        [FromQuery] DateOnly? date,
        [FromQuery] string status,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<EDeliveryStatus>(status, ignoreCase: true, out var deliveryStatus))
            return BadRequest(new { error = $"Invalid status value '{status}'. Allowed values: PENDING, ON_ROUTE, DELIVERED." });

        var resolvedDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);

        var query = new GetDeliveriesByDateAndStatusQuery(resolvedDate, deliveryStatus);
        var result = await deliveryQueryService.Handle(query, cancellationToken);

        var resources = result.Value!.Select(DeliveryResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Get delivery detail",
        Description = "Returns the full detail of a delivery including responsible and vehicle information.",
        OperationId = "GetDeliveryById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Delivery detail returned", typeof(DeliveryDetailResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Delivery not found")]
    public async Task<IActionResult> GetDeliveryById(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var query = new GetDeliveryByIdQuery(id);
        var result = await deliveryQueryService.Handle(query, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error switch
            {
                DeliveryTrackingErrors.DeliveryNotFound => NotFound(new { error = result.Message }),
                _ => BadRequest(new { error = result.Message })
            };
        }

        var resource = DeliveryDetailResourceFromEntityAssembler.ToResourceFromEntity(result.Value!);
        return Ok(resource);
    }

    [HttpPatch("{id:int}/status")]
    [SwaggerOperation(
        Summary = "Update delivery status",
        Description = "Updates the status of a delivery. Only the ON_ROUTE → DELIVERED transition is allowed.",
        OperationId = "UpdateDeliveryStatus")]
    [SwaggerResponse(StatusCodes.Status200OK, "Delivery status updated", typeof(DeliveryDetailResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid status value")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Delivery not found")]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Status transition not allowed")]
    public async Task<IActionResult> UpdateDeliveryStatus(
        [FromRoute] int id,
        [FromBody] UpdateDeliveryStatusResource resource,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<EDeliveryStatus>(resource.Status, ignoreCase: true, out var deliveryStatus))
            return BadRequest(new { error = $"Invalid status value '{resource.Status}'. Allowed values: PENDING, ON_ROUTE, DELIVERED." });

        var command = UpdateDeliveryStatusCommandFromResourceAssembler.ToCommandFromResource(id, resource, deliveryStatus);
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

        var detailResult = await deliveryQueryService.Handle(new GetDeliveryByIdQuery(id), cancellationToken);
        if (detailResult.IsFailure)
            return Ok(new { id = result.Value!.Id, status = result.Value!.Status.ToString().ToUpperInvariant() });

        var detailResource = DeliveryDetailResourceFromEntityAssembler.ToResourceFromEntity(detailResult.Value!);
        return Ok(detailResource);
    }
}
