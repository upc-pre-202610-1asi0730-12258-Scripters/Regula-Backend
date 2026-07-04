using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Scripters.Regula.Platform.InventoryManagement.Application.CommandServices;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Aggregates;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Commands;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Entities;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.InventoryManagement.Domain.Repositories;
using Scripters.Regula.Platform.Shared.Application.Model;
using Scripters.Regula.Platform.Shared.Domain.Repositories;
using Scripters.Regula.Platform.InventoryManagement.Resources;

namespace Scripters.Regula.Platform.InventoryManagement.Application.Internal.CommandServices;

public class InventoryCommandService(
    IInventoryRepository inventoryRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<InventoryManagementMessages> localizer)
    : IInventoryCommandService
{
    private readonly IStringLocalizer<InventoryManagementMessages> _localizer = localizer;

    public async Task<Result<CompanyMovement>> Handle(
        CreateCompanyMovementCommand command,
        CancellationToken cancellationToken)
    {
        var inventory = await inventoryRepository.FindByIdAsync((int)command.InventoryId, cancellationToken);
        if (inventory is null)
            return Result<CompanyMovement>.Failure(InventoryManagementError.InventoryNotFound,
                _localizer[nameof(InventoryManagementError.InventoryNotFound)]);

        if (!inventory.IsCompany())
            return Result<CompanyMovement>.Failure(InventoryManagementError.InvalidInventoryType,
                _localizer[nameof(InventoryManagementError.InvalidInventoryType)]);

        var providerResult = ResolveProviderName(command.MovementType, command.ProviderName);
        if (!providerResult.IsSuccess)
            return Result<CompanyMovement>.Failure(providerResult.Error!, providerResult.Message!);

        var movement = new CompanyMovement(
            command.MovementType,
            command.CylinderType,
            new Quantity(command.Quantity),
            providerResult.Value!,
            new Destination(command.Destination),
            new MovementReason(command.MovementReason),
            new Observation(command.Observation),
            new UserId(command.UserId));

        try
        {
            inventory.RegisterCompanyMovement(movement);
            inventoryRepository.Update(inventory);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<CompanyMovement>.Success(movement);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Insufficient"))
        {
            return Result<CompanyMovement>.Failure(InventoryManagementError.InsufficientStock,
                _localizer[nameof(InventoryManagementError.InsufficientStock)]);
        }
        catch (OperationCanceledException)
        {
            return Result<CompanyMovement>.Failure(InventoryManagementError.OperationCancelled,
                _localizer[nameof(InventoryManagementError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<CompanyMovement>.Failure(InventoryManagementError.DatabaseError,
                _localizer[nameof(InventoryManagementError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<CompanyMovement>.Failure(InventoryManagementError.InternalServerError,
                _localizer[nameof(InventoryManagementError.InternalServerError)]);
        }
    }

    public async Task<Result<DistributorMovement>> Handle(
        CreateDistributorMovementCommand command,
        CancellationToken cancellationToken)
    {
        var inventory = await inventoryRepository.FindByIdAsync((int)command.InventoryId, cancellationToken);
        if (inventory is null)
            return Result<DistributorMovement>.Failure(InventoryManagementError.InventoryNotFound,
                _localizer[nameof(InventoryManagementError.InventoryNotFound)]);

        if (inventory.InventoryType != EInventoryType.Distributor)
            return Result<DistributorMovement>.Failure(InventoryManagementError.InvalidInventoryType,
                _localizer[nameof(InventoryManagementError.InvalidInventoryType)]);

        var providerResult = ResolveProviderName(command.MovementType, command.ProviderName);
        if (!providerResult.IsSuccess)
            return Result<DistributorMovement>.Failure(providerResult.Error!, providerResult.Message!);

        if (command.MovementType == EMovementType.Exit && command.OutboundType is null)
            return Result<DistributorMovement>.Failure(InventoryManagementError.InvalidOutboundType,
                _localizer[nameof(InventoryManagementError.InvalidOutboundType)]);

        var movement = new DistributorMovement(
            command.MovementType,
            command.CylinderType,
            new Quantity(command.Quantity),
            providerResult.Value!,
            command.OutboundType,
            new UserId(command.UserId));

        try
        {
            inventory.RegisterDistributorMovement(movement);
            inventoryRepository.Update(inventory);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<DistributorMovement>.Success(movement);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Insufficient"))
        {
            return Result<DistributorMovement>.Failure(InventoryManagementError.InsufficientStock,
                _localizer[nameof(InventoryManagementError.InsufficientStock)]);
        }
        catch (OperationCanceledException)
        {
            return Result<DistributorMovement>.Failure(InventoryManagementError.OperationCancelled,
                _localizer[nameof(InventoryManagementError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<DistributorMovement>.Failure(InventoryManagementError.DatabaseError,
                _localizer[nameof(InventoryManagementError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<DistributorMovement>.Failure(InventoryManagementError.InternalServerError,
                _localizer[nameof(InventoryManagementError.InternalServerError)]);
        }
    }

    private Result<ProviderName> ResolveProviderName(EMovementType movementType, string? providerName)
    {
        if (movementType == EMovementType.Exit)
            return Result<ProviderName>.Success(new ProviderName(Movement.OwnerProviderName));

        if (string.IsNullOrWhiteSpace(providerName))
            return Result<ProviderName>.Failure(
                InventoryManagementError.InvalidProviderName,
                _localizer[nameof(InventoryManagementError.InvalidProviderName)]);

        var name = new ProviderName(providerName);
        try
        {
            name.Validate();
            return Result<ProviderName>.Success(name);
        }
        catch (ArgumentException)
        {
            return Result<ProviderName>.Failure(
                InventoryManagementError.InvalidProviderName,
                _localizer[nameof(InventoryManagementError.InvalidProviderName)]);
        }
    }
}
