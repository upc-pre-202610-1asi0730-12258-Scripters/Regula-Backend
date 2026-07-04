using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Aggregates;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.InventoryManagement.Domain.Repositories;
using Scripters.Regula.Platform.Shared.Application.Internal.EventHandlers;
using Scripters.Regula.Platform.Shared.Domain.Model.Events;
using Scripters.Regula.Platform.Shared.Domain.Repositories;

namespace Scripters.Regula.Platform.InventoryManagement.Application.Internal.EventHandlers;

/// <summary>
///     Reacts to a new sign-up by provisioning that user's Inventory.
///     Every user in this app is a distributor (no company/enterprise branch
///     is in scope right now), so this always creates a Distributor inventory.
/// </summary>
public class UserRegisteredEventHandler(
    IInventoryRepository inventoryRepository,
    IUnitOfWork unitOfWork) : IEventHandler<UserRegisteredEvent>
{
    public async Task Handle(UserRegisteredEvent domainEvent, CancellationToken cancellationToken)
    {
        var existing = await inventoryRepository.FindByOwnerProfileIdAsync(
            domainEvent.UserId, EInventoryType.Distributor, cancellationToken);

        if (existing is not null)
            return;

        var inventory = new Inventory(new ProfileId(domainEvent.UserId), EInventoryType.Distributor);

        await inventoryRepository.AddAsync(inventory, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
    }
}