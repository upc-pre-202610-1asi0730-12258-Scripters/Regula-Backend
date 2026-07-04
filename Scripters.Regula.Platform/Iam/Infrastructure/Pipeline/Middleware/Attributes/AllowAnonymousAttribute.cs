namespace Scripters.Regula.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>
///     Skips JWT validation for the decorated endpoint.
/// </summary>
/// <remarks>Kevin Lopez</remarks>
[AttributeUsage(AttributeTargets.Method)]
public class AllowAnonymousAttribute : Attribute;
