using Microsoft.AspNetCore.Mvc;
using Scripters.Regula.Platform.CommercialManagement.Application.CommandServices;
using Scripters.Regula.Platform.CommercialManagement.Domain.Errors;
using Scripters.Regula.Platform.CommercialManagement.Interfaces.Rest.Resources;
using Scripters.Regula.Platform.CommercialManagement.Interfaces.Rest.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace Scripters.Regula.Platform.CommercialManagement.Interfaces.Rest;

/// <summary>
/// Controller for managing customer debt operations.
/// </summary>
/// <param name="customerDebtCommandService">The customer debt command service for handling debt-related commands.</param>
[ApiController]
[Route("api/v1/commercial-management/customer-debts")]
[Produces("application/json")]
public class CustomerDebtsController(ICustomerDebtCommandService customerDebtCommandService) : ControllerBase
{
    /// <summary>
    /// Creates a new customer debt.
    /// </summary>
    /// <remarks>
    /// This operation creates a customer debt without creating a sale, payment, stock movement, or debt movement.
    /// </remarks>
    /// <param name="resource">The resource containing details for the new customer debt.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>An <see cref="IActionResult"/> containing the newly created customer debt.</returns>
    /// <response code="201">Returns the newly created customer debt.</response>
    /// <response code="400">If the request is invalid (e.g., invalid debt amount or description).</response>
    /// <response code="404">If the specified customer is not found.</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create customer debt",
        Description = "Creates a customer debt without creating a sale, payment, stock movement or debt movement.",
        OperationId = "CreateCustomerDebt")]
    [SwaggerResponse(StatusCodes.Status201Created, "Customer debt created", typeof(CustomerDebtResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Customer not found")]
    public async Task<IActionResult> CreateCustomerDebt(
        [FromBody] CreateCustomerDebtResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateCustomerDebtCommandFromResourceAssembler.ToCommandFromResource(resource);

        var result = await customerDebtCommandService.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error switch
            {
                CommercialManagementErrors.CustomerNotFound => NotFound(new { error = result.Message }),
                CommercialManagementErrors.InvalidDebtAmount => BadRequest(new { error = result.Message }),
                CommercialManagementErrors.InvalidDebtDescription => BadRequest(new { error = result.Message }),
                _ => BadRequest(new { error = result.Message })
            };
        }

        var customerDebtResource = CustomerDebtResourceFromEntityAssembler.ToResourceFromEntity(result.Value!);

        return StatusCode(StatusCodes.Status201Created, customerDebtResource);
    }

    /// <summary>
    /// Creates a payment for an existing customer debt and updates the remaining balance.
    /// </summary>
    /// <param name="customerDebtId">The unique identifier of the customer debt to pay.</param>
    /// <param name="resource">The resource containing details for the payment.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>An <see cref="IActionResult"/> containing the newly created customer debt payment.</returns>
    /// <response code="201">Returns the newly created customer debt payment.</response>
    /// <response code="400">If the request is invalid (e.g., invalid payment amount, payment exceeds remaining amount, or debt already paid).</response>
    /// <response code="404">If the specified customer debt or customer is not found.</response>
    [HttpPost("{customerDebtId:int}/payments")]
    [SwaggerOperation(
        Summary = "Create customer debt payment",
        Description = "Creates a payment for an existing customer debt and updates the remaining balance.",
        OperationId = "CreateCustomerDebtPayment")]
    [SwaggerResponse(StatusCodes.Status201Created, "Customer debt payment created", typeof(CustomerDebtPaymentResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Customer debt or customer not found")]
    public async Task<IActionResult> CreateCustomerDebtPayment(
        int customerDebtId,
        [FromBody] CreateCustomerDebtPaymentResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateCustomerDebtPaymentCommandFromResourceAssembler.ToCommandFromResource(
            customerDebtId,
            resource);

        var result = await customerDebtCommandService.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error switch
            {
                CommercialManagementErrors.DebtNotFound => NotFound(new { error = result.Message }),
                CommercialManagementErrors.CustomerNotFound => NotFound(new { error = result.Message }),
                CommercialManagementErrors.InvalidPaymentAmount => BadRequest(new { error = result.Message }),
                CommercialManagementErrors.InvalidFullPaymentAmount => BadRequest(new { error = result.Message }),
                CommercialManagementErrors.PaymentExceedsRemainingAmount => BadRequest(new { error = result.Message }),
                CommercialManagementErrors.DebtAlreadyPaid => BadRequest(new { error = result.Message }),
                _ => BadRequest(new { error = result.Message })
            };
        }

        var customerDebtPaymentResource =
            CustomerDebtPaymentResourceFromEntityAssembler.ToResourceFromEntity(result.Value!);

        return StatusCode(StatusCodes.Status201Created, customerDebtPaymentResource);
    }
}