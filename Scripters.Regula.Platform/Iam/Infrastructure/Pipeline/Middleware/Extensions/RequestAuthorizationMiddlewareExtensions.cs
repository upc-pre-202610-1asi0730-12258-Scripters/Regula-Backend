using Scripters.Regula.Platform.Iam.Infrastructure.Pipeline.Middleware.Components;

namespace Scripters.Regula.Platform.Iam.Infrastructure.Pipeline.Middleware.Extensions;

/// <summary>
///     Registers the custom JWT authorization middleware in the ASP.NET Core pipeline.
/// </summary>
/// <remarks>Kevin Lopez</remarks>
public static class RequestAuthorizationMiddlewareExtensions
{
    /// <summary>
    ///     Adds <see cref="RequestAuthorizationMiddleware" /> to the pipeline.
    /// </summary>
    public static IApplicationBuilder UseRequestAuthorization(this IApplicationBuilder builder)
        => builder.UseMiddleware<RequestAuthorizationMiddleware>();
}
