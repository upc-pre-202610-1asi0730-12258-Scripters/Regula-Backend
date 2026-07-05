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
///     Ni este controller ni ninguna capa de aplicación tienen "using Stripe;" —
///     solo Billing.Infrastructure.ExternalServices.StripeGateway lo conoce.
/// </summary>
[ApiController]
[Route("api/v1/subscriptions")]
[Produces("application/json")]
public class SubscriptionsController(
    ISubscriptionCommandService commandService,
    ISubscriptionQueryService queryService) : ControllerBase
{
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
    ///     Recibe eventos de Stripe. Sin [Authorize] a propósito — Stripe no manda
    ///     tu JWT; la autenticidad se valida por la firma Stripe-Signature dentro
    ///     de StripeService, no aquí.
    /// </summary>
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

    private bool TryGetUserId(out long userId)
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return long.TryParse(claim, out userId);
    }
}