using Microsoft.EntityFrameworkCore;
using Scripters.Regula.Platform.Billing.Domain.Model.Aggregates;
using Scripters.Regula.Platform.Billing.Domain.Model.ValueObjects;

namespace Scripters.Regula.Platform.Billing.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyBillingConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Subscription>().ToTable("subscriptions");
        builder.Entity<Subscription>().HasKey(s => s.Id);
        builder.Entity<Subscription>().Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Subscription>().Property(s => s.UserId).IsRequired();

        // Un usuario tiene a lo sumo una fila de suscripción (se actualiza, no se duplica).
        builder.Entity<Subscription>().HasIndex(s => s.UserId).IsUnique();

        builder.Entity<Subscription>().Property(s => s.StripeCustomerId).IsRequired().HasMaxLength(50);
        builder.Entity<Subscription>().Property(s => s.StripeSubscriptionId).HasMaxLength(50);

        builder.Entity<Subscription>()
            .Property(s => s.Status)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion(
                status => status.ToString().ToUpperInvariant(),
                status => Enum.Parse<ESubscriptionStatus>(status, true));
    }
}