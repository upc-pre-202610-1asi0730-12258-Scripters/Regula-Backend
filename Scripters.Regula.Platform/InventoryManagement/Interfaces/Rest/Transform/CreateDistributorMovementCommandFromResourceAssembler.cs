using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Commands;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Resources;

namespace Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Transform;

public static class CreateDistributorMovementCommandFromResourceAssembler
{
    public static CreateDistributorMovementCommand ToCommandFromResource(
        long inventoryId,
        CreateDistributorMovementResource resource)
    {
        if (resource == null)
            throw new ArgumentNullException(nameof(resource));

        EOutboundType? outboundType = resource.OutboundType is null
            ? null
            : Enum.Parse<EOutboundType>(resource.OutboundType, ignoreCase: true);

        return new CreateDistributorMovementCommand(
            inventoryId,
            Enum.Parse<EMovementType>(resource.MovementType, ignoreCase: true),
            Enum.Parse<ECylinderType>(resource.CylinderType, ignoreCase: true),
            resource.Quantity,
            resource.UserId,
            resource.ProviderName,
            outboundType);
    }
}
