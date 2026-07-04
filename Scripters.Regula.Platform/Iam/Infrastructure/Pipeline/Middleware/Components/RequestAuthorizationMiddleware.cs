using Scripters.Regula.Platform.Iam.Application.Internal.OutboundServices;
using Scripters.Regula.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;

namespace Scripters.Regula.Platform.Iam.Infrastructure.Pipeline.Middleware.Components;

/// <summary>
///     Validates the Bearer JWT and stores the authenticated user id in the request context.
/// </summary>
/// <remarks>Kevin Lopez</remarks>
public class RequestAuthorizationMiddleware(RequestDelegate next)
{
    /// <summary>
    ///     Validates the incoming token and forwards the request when authorized.
    /// </summary>
    public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
    {
        var endpoint = context.GetEndpoint();
        var allowAnonymous = endpoint?.Metadata
            .Any(m => m.GetType() == typeof(AllowAnonymousAttribute)) ?? false;

        if (allowAnonymous)
        {
            await next(context);
            return;
        }

        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        var token = authHeader?.Split(' ').Last();

        if (token is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var userId = await tokenService.ValidateToken(token);

        if (userId is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        context.Items["UserId"] = userId.Value;
        await next(context);
    }
}
