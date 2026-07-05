using Microsoft.EntityFrameworkCore;
using Scripters.Regula.Platform.Billing.Domain.Model.Aggregates;
using Scripters.Regula.Platform.Billing.Domain.Repositories;
using Scripters.Regula.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Scripters.Regula.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace Scripters.Regula.Platform.Billing.Infrastructure.Persistence.EFC.Repositories;

public class SubscriptionRepository(AppDbContext context)
    : BaseRepository<Subscription>(context), ISubscriptionRepository
{
    public async Task<Subscription?> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Subscription>()
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
    }

    public async Task<Subscription?> FindByStripeSubscriptionIdAsync(string stripeSubscriptionId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Subscription>()
            .FirstOrDefaultAsync(s => s.StripeSubscriptionId == stripeSubscriptionId, cancellationToken);
    }

    public async Task<Subscription?> FindByStripeCustomerIdAsync(string stripeCustomerId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Subscription>()
            .FirstOrDefaultAsync(s => s.StripeCustomerId == stripeCustomerId, cancellationToken);
    }
}