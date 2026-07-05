using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scripters.Regula.Platform.Billing.Application.CommandServices;
using Scripters.Regula.Platform.Billing.Application.QueryServices;
using Scripters.Regula.Platform.Billing.Domain.Model.Commands;
using Scripters.Regula.Platform.Billing.Domain.Model.Queries;
using Scripters.Regula.Platform.Billing.Interfaces.Rest.Resources;

namespace Scripters.Regula.Platform.Billing.Interfaces.Rest;

/// <summary>
/// Controller for managing user subscriptions.
/// This controller interacts with subscription command and query services.
/// </summary>
/// <remarks>
/// Neither this controller nor any application layer uses "using Stripe;".
/// Only Billing.Infrastructure.ExternalServices.StripeGateway is aware of Stripe specifics.
/// </remarks>
/// <param name="commandService">The subscription command service for handling subscription-related commands.</param>
/// <param name="queryService">The subscription query service for retrieving subscription information.</param>
[ApiController]
[Route("api/v1/billing/subscriptions")]
[Produces("application/json")]
public class SubscriptionsController(
    ISubscriptionCommandService commandService,
    ISubscriptionQueryService queryService) : ControllerBase
{
    /// <summary>
    /// Retrieves the current subscription details for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>An <see cref="IActionResult"/> containing the user's subscription status and end date, or "None" if no active subscription.</returns>
    /// <response code="200">Returns the subscription details or a "None" status.</response>
    /// <response code="401">If the user is not authenticated.</response>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMySubscription(CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var subscription = await queryService.Handle(new GetSubscriptionByUserIdQuery(userId), cancellationToken);

        if (subscription == null)
            return Ok(new SubscriptionResource("None", null));

        return Ok(new SubscriptionResource(subscription.Status.ToString(), subscription.CurrentPeriodEnd));
    }

    /// <summary>
    /// Initiates a Stripe checkout session for a new subscription.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>An <see cref="IActionResult"/> containing the checkout URL or an error message.</returns>
    /// <response code="200">Returns a <see cref="CheckoutResource"/> with the checkout URL.</response>
    /// <response code="400">If the checkout creation fails or the user already has an active subscription.</response>
    /// <response code="401">If the user is not authenticated.</response>
    [HttpPost("checkout")]
    [Authorize]
    public async Task<IActionResult> CreateCheckout(CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var username = User.FindFirst(ClaimTypes.Name)?.Value ?? $"user-{userId}";

        var result = await commandService.Handle(new CreateCheckoutCommand(userId, username), cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Message });

        return Ok(new CheckoutResource(result.Value, AlreadyActive: result.Value == null));
    }

    /// <summary>
    /// Endpoint for receiving webhook events from Stripe.
    /// </summary>
    /// <remarks>
    /// This endpoint intentionally does not have the <see cref="AuthorizeAttribute"/>
    /// because Stripe does not send a JWT. Authenticity is validated by the Stripe-Signature
    /// header within the <c>StripeService</c>, not here.
    /// </remarks>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the success or failure of the webhook processing.</returns>
    /// <response code="200">If the webhook event is processed successfully.</response>
    /// <response code="400">If the webhook processing fails (e.g., invalid signature, malformed payload).</response>
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        var json = await reader.ReadToEndAsync(cancellationToken);

        var result = await commandService.HandleWebhook(json, Request.Headers["Stripe-Signature"]!, cancellationToken);

        if (result.IsFailure)
        {
            Console.WriteLine($"[WEBHOOK FAILED] {result.Error}: {result.Message}");
            return BadRequest(new { error = result.Message });
        }

        return Ok();
    }

    /// <summary>
    /// Attempts to extract the user ID from the current authenticated user's claims.
    /// </summary>
    /// <param name="userId">When this method returns, contains the user ID if successful, or 0 if the operation failed.</param>
    /// <returns><c>true</c> if the user ID was successfully retrieved; otherwise, <c>false</c>.</returns>
    private bool TryGetUserId(out long userId)
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return long.TryParse(claim, out userId);
    }
}