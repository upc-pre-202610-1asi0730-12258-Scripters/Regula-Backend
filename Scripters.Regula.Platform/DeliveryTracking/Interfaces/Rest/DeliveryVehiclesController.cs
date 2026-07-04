using Microsoft.AspNetCore.Mvc;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Repositories;
using Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest;

[ApiController]
[Route("api/v1/delivery-vehicles")]
[Produces("application/json")]
public class DeliveryVehiclesController(IDeliveryVehicleRepository vehicleRepository) : ControllerBase
{
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