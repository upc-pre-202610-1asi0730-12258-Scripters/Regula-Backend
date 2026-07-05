using Microsoft.AspNetCore.Mvc;
using Scripters.Regula.Platform.CommercialManagement.Application.CommandServices;
using Scripters.Regula.Platform.CommercialManagement.Application.QueryServices;
using Scripters.Regula.Platform.CommercialManagement.Domain.Errors;
using Scripters.Regula.Platform.CommercialManagement.Domain.Model.Queries;
using Scripters.Regula.Platform.CommercialManagement.Interfaces.Rest.Resources;
using Scripters.Regula.Platform.CommercialManagement.Interfaces.Rest.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace Scripters.Regula.Platform.CommercialManagement.Interfaces.Rest;

/// <summary>
/// Controller for managing daily sales operations.
/// </summary>
/// <param name="dailySaleCommandService">The daily sale command service for handling daily sale-related commands.</param>
/// <param name="dailySaleQueryService">The daily sale query service for retrieving daily sale information.</param>
[ApiController]
[Route("api/v1/commercial-management/daily-sales")]
[Produces("application/json")]
public class DailySalesController(
    IDailySaleCommandService dailySaleCommandService,
    IDailySaleQueryService dailySaleQueryService) : ControllerBase
{
    /// <summary>
    /// Retrieves all registered daily sales, ordered by creation date in descending order.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="DailySaleResource"/> objects.</returns>
    /// <response code="200">Returns the list of daily sales.</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all daily sales",
        Description = "Gets all registered daily sales ordered by creation date descending.",
        OperationId = "GetAllDailySales")]
    [SwaggerResponse(StatusCodes.Status200OK, "Daily sales found", typeof(IEnumerable<DailySaleResource>))]
    public async Task<IActionResult> GetAllDailySales(CancellationToken cancellationToken)
    {
        var query = new GetAllDailySalesQuery();

        var dailySales = await dailySaleQueryService.Handle(query, cancellationToken);

        var dailySaleResources = dailySales
            .Select(DailySaleResourceFromEntityAssembler.ToResourceFromEntity);

        return Ok(dailySaleResources);
    }

    /// <summary>
    /// Creates a new daily sale.
    /// </summary>
    /// <remarks>
    /// If the payment type is DEBT, this operation also creates the related customer debt.
    /// It does not update inventory stock.
    /// </remarks>
    /// <param name="resource">The resource containing details for the new daily sale.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>An <see cref="IActionResult"/> containing the newly created daily sale.</returns>
    /// <response code="201">Returns the newly created daily sale.</response>
    /// <response code="400">If the request is invalid (e.g., invalid cylinder type, quantity, unit price, payment type, or missing customer for debt sale).</response>
    /// <response code="404">If the specified customer is not found.</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create daily sale",
        Description = "Creates a daily sale. If payment type is DEBT, it also creates the related customer debt. It does not update inventory stock.",
        OperationId = "CreateDailySale")]
    [SwaggerResponse(StatusCodes.Status201Created, "Daily sale created", typeof(DailySaleResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Customer not found")]
    public async Task<IActionResult> CreateDailySale(
        [FromBody] CreateDailySaleResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateDailySaleCommandFromResourceAssembler.ToCommandFromResource(resource);

        var result = await dailySaleCommandService.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error switch
            {
                CommercialManagementErrors.CustomerNotFound => NotFound(new { error = result.Message }),
                CommercialManagementErrors.InvalidCylinderType => BadRequest(new { error = result.Message }),
                CommercialManagementErrors.InvalidSaleQuantity => BadRequest(new { error = result.Message }),
                CommercialManagementErrors.InvalidUnitPrice => BadRequest(new { error = result.Message }),
                CommercialManagementErrors.InvalidPaymentType => BadRequest(new { error = result.Message }),
                CommercialManagementErrors.CustomerRequiredForDebtSale => BadRequest(new { error = result.Message }),
                _ => BadRequest(new { error = result.Message })
            };
        }

        var dailySaleResource = DailySaleResourceFromEntityAssembler.ToResourceFromEntity(result.Value!);

        return StatusCode(StatusCodes.Status201Created, dailySaleResource);
    }
}