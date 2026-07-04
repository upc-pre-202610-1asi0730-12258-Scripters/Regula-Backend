using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Entities;
using Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Resources;

namespace Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Transform;

/// <summary>
///     Converts <see cref="GasCylinderStock" /> entities to REST resources.
/// </summary>
/// <remarks>Kevin Lopez</remarks>
public static class GasCylinderStockResourceFromEntityAssembler
{
    /// <summary>
    ///     Maps a gas cylinder stock entity to its REST resource.
    /// </summary>
    /// <param name="entity">The stock entity to convert.</param>
    /// <returns>The corresponding REST resource.</returns>
    public static GasCylinderStockResource ToResourceFromEntity(GasCylinderStock entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity),
                "GasCylinderStock entity cannot be null when converting to resource.");

        return new GasCylinderStockResource(
            entity.Id,
            entity.CylinderType.ToString(),
            entity.Available.Value,
            entity.InTransit.Value,
            entity.Observed.Value,
            entity.OutOfService.Value);
    }
}
