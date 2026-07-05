using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Scripters.Regula.Platform.Billing.Domain.Model.Aggregates;
using Scripters.Regula.Platform.Billing.Infrastructure.Persistence.EFC.Configuration.Extensions;
using Scripters.Regula.Platform.CommercialManagement.Infrastructure.Persistence.EFC.Configuration.Extensions;
using Scripters.Regula.Platform.CommercialManagement.Domain.Model.Entities;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Aggregates;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Entities;
using Scripters.Regula.Platform.DeliveryTracking.Infrastructure.Persistence.EFC.Configuration.Extensions;
using Scripters.Regula.Platform.Iam.Domain.Model.Aggregates;
using Scripters.Regula.Platform.InventoryManagement.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Scripters.Regula.Platform.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using Scripters.Regula.Platform.Shared.Infrastructure.Persistence.EFC.Interceptors;

namespace Scripters.Regula.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Delivery> Deliveries { get; set; }
    public DbSet<DriverLocation> DriverLocations { get; set; }
    public DbSet<DeliveryResponsible> DeliveryResponsibles { get; set; }
    public DbSet<DeliveryVehicle> DeliveryVehicles { get; set; }
    public DbSet<CommercialDailySale> CommercialDailySales { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyCommercialManagementConfiguration();
        builder.ApplyDeliveryTrackingConfiguration();
        builder.ApplyInventoryManagementConfiguration();
        builder.ApplyBillingConfiguration();

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.UseSnakeCaseNamingConvention();
    }
}