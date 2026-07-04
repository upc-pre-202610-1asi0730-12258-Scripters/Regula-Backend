using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Scripters.Regula.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Scripters.Regula.Platform.InventoryManagement.Application.CommandServices;
using Scripters.Regula.Platform.InventoryManagement.Application.QueryServices;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Queries;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Resources;
using Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Transform;
using Scripters.Regula.Platform.InventoryManagement.Resources;
using Scripters.Regula.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest;

[ApiController]
[Route("api/v1/inventories")]
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

    [HttpGet("my")]
    [SwaggerOperation(Summary = "Get my inventory", OperationId = "GetMyInventory")]
    [SwaggerResponse(StatusCodes.Status200OK, "Inventory", typeof(InventoryResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Inventory not found")]
    public async Task<IActionResult> GetMyInventory(CancellationToken cancellationToken)
    {
        if (HttpContext.Items["UserId"] is not int rawUserId)
            return Unauthorized();

        var inventory = await inventoryQueryService.Handle(
            new GetInventoryByUserIdQuery(new UserId(rawUserId)),
            cancellationToken);

        if (inventory is null)
            return NotFound();

        return Ok(InventoryResourceFromEntityAssembler.ToResourceFromEntity(inventory));
    }

    [AllowAnonymous]
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

    [AllowAnonymous]
    [HttpPost("{inventoryId:long}/company-movements")]
    [SwaggerOperation(Summary = "Register company movement", OperationId = "CreateCompanyMovement")]
    [SwaggerResponse(StatusCodes.Status201Created, "Movement registered", typeof(CompanyMovementResource))]
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
                $"/api/v1/inventories/{inventoryId}/company-movements",
                CompanyMovementResourceFromEntityAssembler.ToResourceFromEntity(movement)));
    }

    [AllowAnonymous]
    [HttpGet("{inventoryId:long}/company-movements")]
    [SwaggerOperation(Summary = "Get company movements", OperationId = "GetCompanyMovements")]
    [SwaggerResponse(StatusCodes.Status200OK, "Company movements", typeof(IEnumerable<CompanyMovementResource>))]
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

        return Ok(movements.Select(CompanyMovementResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [AllowAnonymous]
    [HttpPost("{inventoryId:long}/distributor-movements")]
    [SwaggerOperation(Summary = "Register distributor movement", OperationId = "CreateDistributorMovement")]
    [SwaggerResponse(StatusCodes.Status201Created, "Movement registered", typeof(DistributorMovementResource))]
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
                $"/api/v1/inventories/{inventoryId}/distributor-movements",
                DistributorMovementResourceFromEntityAssembler.ToResourceFromEntity(movement)));
    }

    [AllowAnonymous]
    [HttpGet("{inventoryId:long}/distributor-movements")]
    [SwaggerOperation(Summary = "Get distributor movements", OperationId = "GetDistributorMovements")]
    [SwaggerResponse(StatusCodes.Status200OK, "Distributor movements", typeof(IEnumerable<DistributorMovementResource>))]
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

        return Ok(movements.Select(DistributorMovementResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [AllowAnonymous]
    [HttpGet("{inventoryId:long}/stock")]
    [SwaggerOperation(Summary = "Get stock", OperationId = "GetStock")]
    [SwaggerResponse(StatusCodes.Status200OK, "Stock levels", typeof(IEnumerable<GasCylinderStockResource>))]
    public async Task<IActionResult> GetStock(
        [FromRoute] long inventoryId,
        CancellationToken cancellationToken)
    {
        var stock = await inventoryQueryService.Handle(
            new GetStockByInventoryIdQuery(inventoryId), cancellationToken);

        return Ok(stock.Select(GasCylinderStockResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
