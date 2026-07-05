using Microsoft.AspNetCore.Mvc;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Repositories;
using Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest;

/// <summary>
/// Controller for managing delivery vehicles.
/// </summary>
/// <param name="vehicleRepository">The repository for delivery vehicles.</param>
[ApiController]
[Route("api/v1/delivery-tracking/delivery-vehicles")]
[Produces("application/json")]
public class DeliveryVehiclesController(IDeliveryVehicleRepository vehicleRepository) : ControllerBase
{
    /// <summary>
    /// Retrieves a catalog of all available delivery vehicles.
    /// </summary>
    /// <remarks>
    /// This catalog is read-only for now and is seeded via migration.
    /// </remarks>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="DeliveryVehicleCatalogItem"/> objects.</returns>
    /// <response code="200">Returns the list of delivery vehicles.</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all delivery vehicles",
        Description = "Catalog of vehicles that can be assigned to a delivery. Read-only for now — seeded via migration.",
        OperationId = "GetDeliveryVehicles")]
    [SwaggerResponse(StatusCodes.Status200OK, "Vehicles returned", typeof(IEnumerable<DeliveryVehicleCatalogItem>))]
    public async Task<IActionResult> GetDeliveryVehicles(CancellationToken cancellationToken)
    {
        var vehicles = await vehicleRepository.ListAsync(cancellationToken);

        var resources = vehicles.Select(v => new DeliveryVehicleCatalogItem(v.Id, v.Plate, v.Type, v.Brand));

        return Ok(resources);
    }
}