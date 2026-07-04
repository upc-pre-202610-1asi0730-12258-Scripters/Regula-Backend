using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Entities;
using Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Resources;

namespace Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Transform;

/// <summary>
///     Converts <see cref="CompanyMovement" /> entities to REST resources.
/// </summary>
/// <remarks>Kevin Lopez</remarks>
public static class InventoryCompanyMovementItemResourceFromEntityAssembler
{
    /// <summary>
    ///     Maps a company movement entity to its REST resource.
    /// </summary>
    /// <param name="entity">The company movement entity to convert.</param>
    /// <returns>The corresponding REST resource.</returns>
    public static InventoryCompanyMovementItem ToResourceFromEntity(CompanyMovement entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity),
                "CompanyMovement entity cannot be null when converting to resource.");

        return new InventoryCompanyMovementItem(
            entity.Id,
            entity.Timestamp,
            entity.MovementType.ToString(),
            entity.CylinderType.ToString(),
            entity.Quantity.Value,
            entity.ProfileId.Value,
            entity.ProviderName.Value,
            entity.Destination.Value,
            entity.MovementReason.Value,
            entity.Observation.Value);
    }
}
