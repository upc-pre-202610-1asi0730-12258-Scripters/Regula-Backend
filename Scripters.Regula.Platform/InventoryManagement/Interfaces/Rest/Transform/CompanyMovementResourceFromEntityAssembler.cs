using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Entities;
using Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Resources;

namespace Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Transform;

/// <summary>
///     Converts <see cref="CompanyMovement" /> entities to REST resources.
/// </summary>
/// <remarks>Kevin Lopez</remarks>
public static class CompanyMovementResourceFromEntityAssembler
{
    /// <summary>
    ///     Maps a company movement entity to its REST resource.
    /// </summary>
    public static CompanyMovementResource ToResourceFromEntity(CompanyMovement entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity),
                "CompanyMovement entity cannot be null when converting to resource.");

        return new CompanyMovementResource(
            entity.Id,
            entity.Timestamp,
            entity.MovementType.ToString(),
            entity.CylinderType.ToString(),
            entity.Quantity.Value,
            entity.UserId.Value,
            entity.ProviderName.Value,
            entity.Destination.Value,
            entity.MovementReason.Value,
            entity.Observation.Value);
    }
}
