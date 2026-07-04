using Scripters.Regula.Platform.Iam.Domain.Model.Aggregates;

namespace Scripters.Regula.Platform.Iam.Application.Internal.OutboundServices;

/// <summary>
///     Outbound port for JWT generation and validation.
/// </summary>
/// <remarks>Kevin Lopez</remarks>
public interface ITokenService
{
    string GenerateToken(User user);

    Task<int?> ValidateToken(string token);
}
