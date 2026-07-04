using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Entities;
using Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Resources;

namespace Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Transform;

/// <summary>
///     Converts <see cref="DistributorMovement" /> entities to REST resources.
/// </summary>
/// <remarks>Kevin Lopez</remarks>
public static class InventoryDistributorMovementItemResourceFromEntityAssembler
{
    /// <summary>
    ///     Maps a distributor movement entity to its REST resource.
    /// </summary>
    /// <param name="entity">The distributor movement entity to convert.</param>
    /// <returns>The corresponding REST resource.</returns>
    public static InventoryDistributorMovementItem ToResourceFromEntity(DistributorMovement entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity),
                "DistributorMovement entity cannot be null when converting to resource.");

        return new InventoryDistributorMovementItem(
            entity.Id,
            entity.Timestamp,
            entity.MovementType.ToString(),
            entity.CylinderType.ToString(),
            entity.Quantity.Value,
            entity.ProfileId.Value,
            entity.ProviderName.Value,
            entity.OutboundType?.ToString());
    }
}
