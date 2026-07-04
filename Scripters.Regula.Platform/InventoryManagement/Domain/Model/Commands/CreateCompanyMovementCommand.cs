using Scripters.Regula.Platform.InventoryManagement.Domain.Model.ValueObjects;

namespace Scripters.Regula.Platform.InventoryManagement.Domain.Model.Commands;

public record CreateCompanyMovementCommand(
    long          InventoryId,
    EMovementType MovementType,
    ECylinderType CylinderType,
    int           Quantity,
    int           UserId,
    string?       ProviderName,
    string        Destination,
    string        MovementReason,
    string        Observation);
