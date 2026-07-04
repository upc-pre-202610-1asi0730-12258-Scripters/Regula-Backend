using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Scripters.Regula.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>
///     Ensures the authenticated user id is present in <c>HttpContext.Items</c>.
/// </summary>
/// <remarks>Kevin Lopez</remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    /// <inheritdoc />
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata
            .OfType<AllowAnonymousAttribute>().Any();

        if (allowAnonymous)
            return;

        if (context.HttpContext.Items["UserId"] is null)
            context.Result = new UnauthorizedResult();
    }
}
