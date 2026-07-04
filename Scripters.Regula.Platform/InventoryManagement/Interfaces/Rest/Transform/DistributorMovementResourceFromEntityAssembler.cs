using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Entities;
using Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Resources;

namespace Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Transform;

/// <summary>
///     Converts <see cref="DistributorMovement" /> entities to REST resources.
/// </summary>
/// <remarks>Kevin Lopez</remarks>
public static class DistributorMovementResourceFromEntityAssembler
{
    /// <summary>
    ///     Maps a distributor movement entity to its REST resource.
    /// </summary>
    public static DistributorMovementResource ToResourceFromEntity(DistributorMovement entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity),
                "DistributorMovement entity cannot be null when converting to resource.");

        return new DistributorMovementResource(
            entity.Id,
            entity.Timestamp,
            entity.MovementType.ToString(),
            entity.CylinderType.ToString(),
            entity.Quantity.Value,
            entity.UserId.Value,
            entity.ProviderName.Value,
            entity.OutboundType?.ToString());
    }
}
