using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Scripters.Regula.Platform.Iam.Application.Internal.CommandServices;
using Scripters.Regula.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Scripters.Regula.Platform.Iam.Interfaces.Rest.Resources;
using Scripters.Regula.Platform.Iam.Interfaces.Rest.Transform;

namespace Scripters.Regula.Platform.Iam.Interfaces.Rest;

[ApiController]
[Route("/api/v1/authentication")]
[Produces(MediaTypeNames.Application.Json)]
public class AuthenticationController(IUserCommandService userCommandService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] SignUpResource resource)
    {
        var command = SignUpCommandFromResourceAssembler.ToCommandFromResource(resource);
        await userCommandService.Handle(command);
        return Ok(new { message = "User created successfully" });
    }
    
    [AllowAnonymous]
    [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignInResource resource)
        {
        var command = SignInCommandFromResourceAssembler.ToCommandFromResource(resource);
        var (user, token) = await userCommandService.Handle(command);
        var authenticatedUserResource = AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(user, token);
        return Ok(authenticatedUserResource);
    }
}