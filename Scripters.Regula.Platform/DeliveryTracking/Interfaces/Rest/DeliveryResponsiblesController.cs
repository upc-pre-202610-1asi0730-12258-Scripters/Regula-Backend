using Microsoft.AspNetCore.Mvc;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Repositories;
using Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest;

[ApiController]
[Route("api/v1/delivery-responsibles")]
[Produces("application/json")]
public class DeliveryResponsiblesController(IDeliveryResponsibleRepository responsibleRepository) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all delivery responsibles",
        Description = "Catalog of people who can be assigned as responsible for a delivery. Read-only for now — seeded via migration.",
        OperationId = "GetDeliveryResponsibles")]
    [SwaggerResponse(StatusCodes.Status200OK, "Responsibles returned", typeof(IEnumerable<DeliveryResponsibleCatalogItem>))]
    public async Task<IActionResult> GetDeliveryResponsibles(CancellationToken cancellationToken)
    {
        var responsibles = await responsibleRepository.ListAsync(cancellationToken);

        var resources = responsibles.Select(r => new DeliveryResponsibleCatalogItem(r.Id, r.Name));

        return Ok(resources);
    }
}