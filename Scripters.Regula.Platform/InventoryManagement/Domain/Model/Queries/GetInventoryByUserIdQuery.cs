namespace Scripters.Regula.Platform.InventoryManagement.Domain.Model.Queries;

using Scripters.Regula.Platform.InventoryManagement.Domain.Model.ValueObjects;

/// <summary>
///     Query to retrieve the inventory owned by a user.
/// </summary>
/// <param name="UserId">The authenticated user identifier.</param>
/// <remarks>Kevin Lopez</remarks>
public record GetInventoryByUserIdQuery(UserId UserId);
