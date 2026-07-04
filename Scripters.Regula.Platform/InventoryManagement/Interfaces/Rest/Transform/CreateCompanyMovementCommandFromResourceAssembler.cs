using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Commands;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Resources;

namespace Scripters.Regula.Platform.InventoryManagement.Interfaces.Rest.Transform;

public static class CreateCompanyMovementCommandFromResourceAssembler
{
    public static CreateCompanyMovementCommand ToCommandFromResource(
        long inventoryId,
        CreateCompanyMovementResource resource)
    {
        if (resource == null)
            throw new ArgumentNullException(nameof(resource));

        return new CreateCompanyMovementCommand(
            inventoryId,
            Enum.Parse<EMovementType>(resource.MovementType, ignoreCase: true),
            Enum.Parse<ECylinderType>(resource.CylinderType, ignoreCase: true),
            resource.Quantity,
            resource.UserId,
            resource.ProviderName,
            resource.Destination,
            resource.MovementReason,
            resource.Observation);
    }
}
