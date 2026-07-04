using Cortex.Mediator;
using Scripters.Regula.Platform.Iam.Application.Commands;
using Scripters.Regula.Platform.Iam.Application.Internal.OutboundServices;
using Scripters.Regula.Platform.Iam.Domain.Model.Aggregates;
using Scripters.Regula.Platform.Iam.Domain.Repositories;
using Scripters.Regula.Platform.Shared.Domain.Model.Events;
using Scripters.Regula.Platform.Shared.Domain.Repositories;

namespace Scripters.Regula.Platform.Iam.Application.Internal.CommandServices;

public class UserCommandService(
    IUserRepository userRepository,
    IHashingService hashingService,
    ITokenService tokenService, 
    IUnitOfWork unitOfWork,
    IMediator mediator)
    : IUserCommandService
{
    public async Task Handle(SignUpCommand command)
    {
        var existingUser = await userRepository.FindByUsernameAsync(command.Username);
        if (existingUser != null)
        {
            throw new Exception("Username already exists.");
        }

        var hashedPassword = hashingService.HashPassword(command.Password);
        var newUser = new User(command.Username, hashedPassword);
        
        await userRepository.AddAsync(newUser);
        await unitOfWork.CompleteAsync();

        // newUser.Id solo existe después de CompleteAsync (autoincrement).
        // Se publica después del commit del User: InventoryManagement crea su
        // propio Inventory en una transacción separada al reaccionar a esto
        // (consistencia eventual, no atómica con la creación del User).
        await mediator.PublishAsync(new UserRegisteredEvent(newUser.Id));
    }
    
 
    public async Task<(User user, string token)> Handle(SignInCommand command)
    {
        var user = await userRepository.FindByUsernameAsync(command.Username);
        if (user == null || !hashingService.VerifyPassword(command.Password, user.PasswordHash))
        {
            throw new Exception("Invalid username or password.");
        }

        var token = tokenService.GenerateToken(user);
        return (user, token);
    } 
}