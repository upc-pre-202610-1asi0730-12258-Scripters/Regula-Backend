namespace Scripters.Regula.Platform.Shared.Domain.Model.Events;

/// <summary>
///     Integration event published when a new user finishes sign-up.
///     Lives in the Shared kernel (not in Iam) so that other bounded contexts
///     (e.g. InventoryManagement) can react to it without Iam having to know
///     who's listening, and without those contexts depending on Iam's own
///     types (User, SignUpCommand, etc.) — only this primitive-only event.
/// </summary>
/// <param name="UserId">The newly created user's id (Iam.User.Id).</param>
public record UserRegisteredEvent(long UserId) : IEvent;