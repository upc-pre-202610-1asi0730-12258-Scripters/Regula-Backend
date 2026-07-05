using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Scripters.Regula.Platform.Iam.Application.Internal.CommandServices;
using Scripters.Regula.Platform.Iam.Interfaces.Rest.Resources;
using Scripters.Regula.Platform.Iam.Interfaces.Rest.Transform;

namespace Scripters.Regula.Platform.Iam.Interfaces.Rest;

/// <summary>
/// Controller for handling authentication-related operations.
/// </summary>
/// <param name="userCommandService">The user command service for handling user-related commands.</param>
[ApiController]
[Route("api/v1/iam/authentication")]
[Produces(MediaTypeNames.Application.Json)]
public class AuthenticationController(IUserCommandService userCommandService) : ControllerBase
{
    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="resource">The sign-up request resource containing user details.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the sign-up operation.</returns>
    /// <response code="200">Returns a success message if the user is created successfully.</response>
    /// <response code="400">If the input resource is invalid or user already exists.</response>
    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] SignUpResource resource)
    {
        var command = SignUpCommandFromResourceAssembler.ToCommandFromResource(resource);
        await userCommandService.Handle(command);
        return Ok(new { message = "User created successfully" });
    }
    
    /// <summary>
    /// Authenticates an existing user and provides an authentication token.
    /// </summary>
    /// <param name="resource">The sign-in request resource containing user credentials.</param>
    /// <returns>An <see cref="IActionResult"/> containing the authenticated user details and token.</returns>
    /// <response code="200">Returns the authenticated user resource with a JWT token.</response>
    /// <response code="401">If the credentials are invalid.</response>
    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] SignInResource resource)
    {
        var command = SignInCommandFromResourceAssembler.ToCommandFromResource(resource);
        var (user, token) = await userCommandService.Handle(command);
        var authenticatedUserResource = AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(user, token);
        return Ok(authenticatedUserResource);
    }
}