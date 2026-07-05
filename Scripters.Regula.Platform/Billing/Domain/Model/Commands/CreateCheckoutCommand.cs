namespace Scripters.Regula.Platform.Billing.Domain.Model.Commands;

/// <param name="UserId">Iam.User.Id — el ProfileId del JWT.</param>
/// <param name="Username">Solo para identificar al cliente en el Dashboard de Stripe (no hay email en Iam.User).</param>
public record CreateCheckoutCommand(long UserId, string Username);