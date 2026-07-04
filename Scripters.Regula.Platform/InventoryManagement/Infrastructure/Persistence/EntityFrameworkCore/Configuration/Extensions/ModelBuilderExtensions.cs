using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Aggregates;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.Entities;
using Scripters.Regula.Platform.InventoryManagement.Domain.Model.ValueObjects;

namespace Scripters.Regula.Platform.InventoryManagement.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    private static readonly ValueConverter<ProviderName, string> ProviderNameConverter =
        new(v => v.Value, v => new ProviderName(v));

    private static readonly ValueConverter<UserId, int> UserIdConverter =
        new(v => v.Value, v => new UserId(v));

    private static readonly ValueConverter<Quantity, int> QuantityConverter =
        new(v => v.Value, v => new Quantity(v));

    private static readonly ValueConverter<Destination, string> DestinationConverter =
        new(v => v.Value, v => new Destination(v));

    private static readonly ValueConverter<MovementReason, string> MovementReasonConverter =
        new(v => v.Value, v => new MovementReason(v));

    private static readonly ValueConverter<Observation, string> ObservationConverter =
        new(v => v.Value, v => new Observation(v));

    public static void ApplyInventoryManagementConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Inventory>(entity =>
        {
            entity.ToTable("inventories");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            entity.Property(x => x.UserId)
                .HasConversion(UserIdConverter)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(x => x.InventoryType)
                .HasConversion<string>()
                .HasColumnName("inventory_type")
                .HasMaxLength(20)
                .IsRequired();

            entity.HasIndex(x => new
            {
                x.UserId,
                x.InventoryType
            }).IsUnique();

            entity.Ignore(x => x.StockSummary);
            entity.Ignore(x => x.MovementHistory);

            entity.HasMany(x => x.StockItems)
                .WithOne()
                .HasForeignKey("inventory_id")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.Movements)
                .WithOne()
                .HasForeignKey("inventory_id")
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<GasCylinderStock>(entity =>
        {
            entity.ToTable("gas_cylinder_stocks");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            entity.Property(x => x.CylinderType)
                .HasConversion<string>()
                .HasMaxLength(10)
                .IsRequired();

            entity.Property(x => x.Available)
                .HasConversion(QuantityConverter)
                .HasColumnName("available")
                .IsRequired();

            entity.Property(x => x.InTransit)
                .HasConversion(QuantityConverter)
                .HasColumnName("in_transit")
                .IsRequired();

            entity.Property(x => x.Observed)
                .HasConversion(QuantityConverter)
                .HasColumnName("observed")
                .IsRequired();

            entity.Property(x => x.OutOfService)
                .HasConversion(QuantityConverter)
                .HasColumnName("out_of_service")
                .IsRequired();
        });

        builder.Entity<Movement>(entity =>
        {
            entity.UseTptMappingStrategy();

            entity.ToTable("movements");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            entity.Property(x => x.Timestamp)
                .IsRequired();

            entity.Property(x => x.MovementType)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.CylinderType)
                .HasConversion<string>()
                .HasMaxLength(10)
                .IsRequired();

            entity.Property(x => x.Quantity)
                .HasConversion(QuantityConverter)
                .HasColumnName("quantity")
                .IsRequired();

            entity.Property(x => x.ProviderName)
                .HasConversion(ProviderNameConverter)
                .HasColumnName("provider_name")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.UserId)
                .HasConversion(UserIdConverter)
                .HasColumnName("user_id")
                .IsRequired();
        });

        builder.Entity<CompanyMovement>(entity =>
        {
            entity.ToTable("company_movements");

            entity.Property(x => x.Destination)
                .HasConversion(DestinationConverter)
                .HasColumnName("destination")
                .HasMaxLength(200);

            entity.Property(x => x.MovementReason)
                .HasConversion(MovementReasonConverter)
                .HasColumnName("movement_reason")
                .HasMaxLength(300);

            entity.Property(x => x.Observation)
                .HasConversion(ObservationConverter)
                .HasColumnName("observation")
                .HasMaxLength(500);
        });

        builder.Entity<DistributorMovement>(entity =>
        {
            entity.ToTable("distributor_movements");

            entity.Property(x => x.OutboundType)
                .HasConversion<string>()
                .HasColumnName("outbound_type")
                .HasMaxLength(30);
        });
    }
}
