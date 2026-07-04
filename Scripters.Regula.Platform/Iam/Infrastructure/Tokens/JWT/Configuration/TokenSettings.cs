namespace Scripters.Regula.Platform.Iam.Infrastructure.Tokens.Jwt.Configuration;

/// <summary>
///     JWT signing configuration bound from appsettings.
/// </summary>
/// <remarks>Kevin Lopez</remarks>
public class TokenSettings
{
    public required string Secret { get; set; }
}
