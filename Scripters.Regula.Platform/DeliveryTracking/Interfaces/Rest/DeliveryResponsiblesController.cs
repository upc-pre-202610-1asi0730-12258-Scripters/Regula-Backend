using Microsoft.AspNetCore.Mvc;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Repositories;
using Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Scripters.Regula.Platform.DeliveryTracking.Interfaces.Rest;

/// <summary>
/// Controller for managing delivery responsibles.
/// </summary>
/// <param name="responsibleRepository">The repository for delivery responsibles.</param>
[ApiController]
[Route("api/v1/delivery-tracking/delivery-responsibles")]
[Produces("application/json")]
public class DeliveryResponsiblesController(IDeliveryResponsibleRepository responsibleRepository) : ControllerBase
{
    /// <summary>
    /// Retrieves a catalog of all available delivery responsibles.
    /// </summary>
    /// <remarks>
    /// This catalog is read-only for now and is seeded via migration.
    /// </remarks>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="DeliveryResponsibleCatalogItem"/> objects.</returns>
    /// <response code="200">Returns the list of delivery responsibles.</response>
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