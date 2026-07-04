namespace Scripters.Regula.Platform.InventoryManagement.Domain.Model.ValueObjects;

/// <summary>
///     External user identifier obtained from the authenticated JWT.
/// </summary>
/// <param name="Value">The user identifier.</param>
/// <remarks>Kevin Lopez</remarks>
public record UserId(int Value)
{
    public UserId() : this(0)
    {
    }
}
