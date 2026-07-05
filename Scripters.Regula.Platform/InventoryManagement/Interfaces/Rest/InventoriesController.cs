using System.Net.Mime;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Scripters.Regula.Platform.InventoryManagement.Application.CommandServices;
using Scripters.Regula.Platform.InventoryManagement.Application.QueryServices;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Queries;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Resources;
using Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Transform;
using Scripters.Regula.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Scripters.Regula.Platform.InventoryManagement.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest;

/// <summary>
/// Controller for managing inventory operations.
/// The route uses the bounded context prefix: api/v1/{bounded-context}/{resource}.
/// </summary>
/// <param name="inventoryCommandService">The inventory command service for handling inventory-related commands.</param>
/// <param name="inventoryQueryService">The inventory query service for retrieving inventory information.</param>
/// <param name="errorLocalizer">The string localizer for inventory management error messages.</param>
/// <param name="problemDetailsFactory">The factory for creating problem details responses.</param>
[ApiController]
[Route("api/v1/inventory-management/inventories")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Inventory Management Endpoints.")]
public class InventoriesController(
    IInventoryCommandService inventoryCommandService,
    IInventoryQueryService inventoryQueryService,
    IStringLocalizer<InventoryManagementMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    private readonly IStringLocalizer<InventoryManagementMessages> _errorLocalizer = errorLocalizer;
    private readonly ProblemDetailsFactory _problemDetailsFactory = problemDetailsFactory;

    /// <summary>
    /// Retrieves the authenticated user's own distributor inventory.
    /// </summary>
    /// <remarks>
    /// The inventory is resolved via the ProfileId (NameIdentifier claim) carried in the JWT; no inventoryId is needed.
    /// </remarks>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>An <see cref="IActionResult"/> containing the inventory details.</returns>
    /// <response code="200">Returns the inventory.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="404">If the inventory is not found for this user.</response>
    [HttpGet("me")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Get the authenticated user's own distributor inventory",
        Description = "Resolves the inventory via the ProfileId (NameIdentifier claim) carried in the JWT — no inventoryId needed.",
        OperationId = "GetMyInventory")]
    [SwaggerResponse(StatusCodes.Status200OK, "Inventory", typeof(InventoryResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Inventory not found for this user")]
    public async Task<IActionResult> GetMyInventory(CancellationToken cancellationToken)
    {
        var profileIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(profileIdClaim, out var profileId))
            return Unauthorized();

        var inventory = await inventoryQueryService.Handle(
            new GetInventoryByOwnerProfileIdQuery(profileId, EInventoryType.Distributor), cancellationToken);

        return InventoryManagementActionResultAssembler.ToActionResultFromGetInventoryByIdResult(
            this, inventory, _errorLocalizer, _problemDetailsFactory,
            found => Ok(InventoryResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }

    /// <summary>
    /// Retrieves inventory details by a specific inventory ID.
    /// </summary>
    /// <param name="inventoryId">The unique identifier of the inventory.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>An <see cref="IActionResult"/> containing the inventory details.</returns>
    /// <response code="200">Returns the inventory.</response>
    /// <response code="404">If the inventory is not found.</response>
    [HttpGet("{inventoryId:long}")]
    [SwaggerOperation(Summary = "Get inventory by id", OperationId = "GetInventoryById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Inventory", typeof(InventoryResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Inventory not found")]
    public async Task<IActionResult> GetInventoryById(
        [FromRoute] long inventoryId,
        CancellationToken cancellationToken)
    {
        var inventory = await inventoryQueryService.Handle(
            new GetInventoryByIdQuery(inventoryId), cancellationToken);

        return InventoryManagementActionResultAssembler.ToActionResultFromGetInventoryByIdResult(
            this, inventory, _errorLocalizer, _problemDetailsFactory,
            found => Ok(InventoryResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }

    /// <summary>
    /// Registers a new company movement for a specific inventory.
    /// </summary>
    /// <param name="inventoryId">The unique identifier of the inventory.</param>
    /// <param name="resource">The resource containing details for the company movement.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>An <see cref="IActionResult"/> containing the registered company movement item.</returns>
    /// <response code="201">Returns the registered movement.</response>
    /// <response code="400">If the input is invalid.</response>
    [HttpPost("{inventoryId:long}/company-movements")]
    [SwaggerOperation(Summary = "Register company movement", OperationId = "CreateCompanyMovement")]
    [SwaggerResponse(StatusCodes.Status201Created, "Movement registered", typeof(InventoryCompanyMovementItem))]
    public async Task<IActionResult> CreateCompanyMovement(
        [FromRoute] long inventoryId,
        [FromBody] CreateCompanyMovementResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateCompanyMovementCommandFromResourceAssembler.ToCommandFromResource(inventoryId, resource);
        var result = await inventoryCommandService.Handle(command, cancellationToken);

        return InventoryManagementActionResultAssembler.ToActionResultFromCommandResult(
            this, result, _errorLocalizer, _problemDetailsFactory,
            movement => Created(
                $"/api/v1/inventory-management/inventories/{inventoryId}/company-movements",
                InventoryItemFromEntityAssembler.ToCompanyMovementItem(movement)));
    }

    /// <summary>
    /// Retrieves company movements for a specific inventory, optionally filtered by movement type.
    /// </summary>
    /// <param name="inventoryId">The unique identifier of the inventory.</param>
    /// <param name="movementType">Optional. The type of movement to filter by (e.g., IN, OUT).</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="InventoryCompanyMovementItem"/> objects.</returns>
    /// <response code="200">Returns the list of company movements.</response>
    [HttpGet("{inventoryId:long}/company-movements")]
    [SwaggerOperation(Summary = "Get company movements", OperationId = "GetCompanyMovements")]
    [SwaggerResponse(StatusCodes.Status200OK, "Company movements", typeof(IEnumerable<InventoryCompanyMovementItem>))]
    public async Task<IActionResult> GetCompanyMovements(
        [FromRoute] long inventoryId,
        [FromQuery] string? movementType,
        CancellationToken cancellationToken)
    {
        EMovementType? type = movementType is null
            ? null
            : Enum.Parse<EMovementType>(movementType, ignoreCase: true);

        var movements = await inventoryQueryService.Handle(
            new GetCompanyMovementsByInventoryIdQuery(inventoryId, type), cancellationToken);

        return Ok(movements.Select(InventoryItemFromEntityAssembler.ToCompanyMovementItem));
    }

    /// <summary>
    /// Registers a new distributor movement for a specific inventory.
    /// </summary>
    /// <param name="inventoryId">The unique identifier of the inventory.</param>
    /// <param name="resource">The resource containing details for the distributor movement.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>An <see cref="IActionResult"/> containing the registered distributor movement item.</returns>
    /// <response code="201">Returns the registered movement.</response>
    /// <response code="400">If the input is invalid.</response>
    [HttpPost("{inventoryId:long}/distributor-movements")]
    [SwaggerOperation(Summary = "Register distributor movement", OperationId = "CreateDistributorMovement")]
    [SwaggerResponse(StatusCodes.Status201Created, "Movement registered", typeof(InventoryDistributorMovementItem))]
    public async Task<IActionResult> CreateDistributorMovement(
        [FromRoute] long inventoryId,
        [FromBody] CreateDistributorMovementResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateDistributorMovementCommandFromResourceAssembler.ToCommandFromResource(inventoryId, resource);
        var result = await inventoryCommandService.Handle(command, cancellationToken);

        return InventoryManagementActionResultAssembler.ToActionResultFromCommandResult(
            this, result, _errorLocalizer, _problemDetailsFactory,
            movement => Created(
                $"/api/v1/inventory-management/inventories/{inventoryId}/distributor-movements",
                InventoryItemFromEntityAssembler.ToDistributorMovementItem(movement)));
    }

    /// <summary>
    /// Retrieves distributor movements for a specific inventory, optionally filtered by movement type.
    /// </summary>
    /// <param name="inventoryId">The unique identifier of the inventory.</param>
    /// <param name="movementType">Optional. The type of movement to filter by (e.g., IN, OUT).</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="InventoryDistributorMovementItem"/> objects.</returns>
    /// <response code="200">Returns the list of distributor movements.</response>
    [HttpGet("{inventoryId:long}/distributor-movements")]
    [SwaggerOperation(Summary = "Get distributor movements", OperationId = "GetDistributorMovements")]
    public async Task<IActionResult> GetDistributorMovements(
        [FromRoute] long inventoryId,
        [FromQuery] string? movementType,
        CancellationToken cancellationToken)
    {
        EMovementType? type = movementType is null
            ? null
            : Enum.Parse<EMovementType>(movementType, ignoreCase: true);

        var movements = await inventoryQueryService.Handle(
            new GetDistributorMovementsByInventoryIdQuery(inventoryId, type), cancellationToken);

        return Ok(movements.Select(InventoryItemFromEntityAssembler.ToDistributorMovementItem));
    }

    /// <summary>
    /// Retrieves inventory items (stock) for a specific inventory.
    /// </summary>
    /// <remarks>
    /// The resource is named "inventory-items" to align with the domain's <c>InventoryStockItem</c>
    /// and avoid the generic term "stock," which doesn't specify the resource's form.
    /// </remarks>
    /// <param name="inventoryId">The unique identifier of the inventory.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A list of inventory stock items.</returns>
    /// <response code="200">Returns the list of inventory items.</response>
    [HttpGet("{inventoryId:long}/inventory-items")]
    [SwaggerOperation(Summary = "Get inventory items (stock)", OperationId = "GetInventoryItems")]
    public async Task<IActionResult> GetInventoryItems(
        [FromRoute] long inventoryId,
        CancellationToken cancellationToken)
    {
        var stock = await inventoryQueryService.Handle(
            new GetStockByInventoryIdQuery(inventoryId), cancellationToken);

        return Ok(stock.Select(InventoryItemFromEntityAssembler.ToStockItem));
    }
}