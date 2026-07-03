using Microsoft.EntityFrameworkCore;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Aggregates;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Entities;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.ValueObjects;

namespace Scripters.Regula.Platform.DeliveryTracking.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyDeliveryTrackingConfiguration(this ModelBuilder builder)
    {
        builder.Entity<DeliveryResponsible>().ToTable("delivery_responsibles");
        builder.Entity<DeliveryResponsible>().HasKey(r => r.Id);
        builder.Entity<DeliveryResponsible>().Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<DeliveryResponsible>().Property(r => r.Name).IsRequired().HasMaxLength(100);

        builder.Entity<DeliveryResponsible>().HasData(
            new { Id = 1, Name = "Juan López" },
            new { Id = 2, Name = "Pedro Salas" },
            new { Id = 3, Name = "Ana Gómez" },
            new { Id = 4, Name = "Carlos Ruiz" },
            new { Id = 5, Name = "Luis Torres" },
            new { Id = 6, Name = "Raúl Méndez" });

        builder.Entity<DeliveryVehicle>().ToTable("delivery_vehicles");
        builder.Entity<DeliveryVehicle>().HasKey(v => v.Id);
        builder.Entity<DeliveryVehicle>().Property(v => v.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<DeliveryVehicle>().Property(v => v.Plate).IsRequired().HasMaxLength(20);
        builder.Entity<DeliveryVehicle>().Property(v => v.Type).IsRequired().HasMaxLength(50);
        builder.Entity<DeliveryVehicle>().Property(v => v.Brand).IsRequired().HasMaxLength(50);

        builder.Entity<DeliveryVehicle>().HasData(
            new { Id = 1, Plate = "A38-210", Type = "Moto", Brand = "Honda" },
            new { Id = 2, Plate = "C5R-982", Type = "Camioneta", Brand = "Toyota" },
            new { Id = 3, Plate = "B12-400", Type = "Moto", Brand = "Yamaha" },
            new { Id = 4, Plate = "XYZ-787", Type = "Camión", Brand = "Hino" },
            new { Id = 5, Plate = "D45-001", Type = "Moto", Brand = "Honda" },
            new { Id = 6, Plate = "X1W-445", Type = "Moto", Brand = "Bajaj" });

        builder.Entity<Delivery>().ToTable("deliveries");
        builder.Entity<Delivery>().HasKey(d => d.Id);
        builder.Entity<Delivery>().Property(d => d.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Delivery>().Property(d => d.DriverId).IsRequired();
        builder.Entity<Delivery>().Property(d => d.ResponsibleId).IsRequired();
        builder.Entity<Delivery>().Property(d => d.VehicleId).IsRequired();
        builder.Entity<Delivery>().Property(d => d.ItemCount).IsRequired();
        builder.Entity<Delivery>().Property(d => d.Cargo).IsRequired().HasMaxLength(200);
        builder.Entity<Delivery>().Property(d => d.Destination).IsRequired().HasMaxLength(200);
        builder.Entity<Delivery>().Property(d => d.ScheduledTime).IsRequired();
        builder.Entity<Delivery>().Property(d => d.DeliveredAt).HasMaxLength(5);

        builder.Entity<Delivery>()
            .Property(d => d.Status)
            .IsRequired()
            .HasConversion(
                status => status.ToString().ToUpperInvariant(),
                status => Enum.Parse<EDeliveryStatus>(status, true));

        builder.Entity<Delivery>()
            .HasOne(d => d.Responsible)
            .WithMany()
            .HasForeignKey(d => d.ResponsibleId);

        builder.Entity<Delivery>()
            .HasOne(d => d.Vehicle)
            .WithMany()
            .HasForeignKey(d => d.VehicleId);

        builder.Entity<Delivery>().HasData(
            new { Id = 1, DriverId = 101, ResponsibleId = 1, VehicleId = 1, ItemCount = 3, Cargo = "3 balones", Destination = "Entrega Centro", ScheduledTime = new DateTime(2026, 6, 16, 14, 25, 0), Status = EDeliveryStatus.OnRoute },
            new { Id = 2, DriverId = 102, ResponsibleId = 2, VehicleId = 2, ItemCount = 2, Cargo = "2 balones", Destination = "Av. Los Pinos 456", ScheduledTime = new DateTime(2026, 6, 16, 13, 45, 0), Status = EDeliveryStatus.Delivered, DeliveredAt = "13:45" },
            new { Id = 3, DriverId = 103, ResponsibleId = 3, VehicleId = 3, ItemCount = 1, Cargo = "1 balón", Destination = "Jr. Las Flores 123", ScheduledTime = new DateTime(2026, 6, 16, 12, 5, 0), Status = EDeliveryStatus.Delivered, DeliveredAt = "12:05" },
            new { Id = 4, DriverId = 104, ResponsibleId = 4, VehicleId = 4, ItemCount = 10, Cargo = "10 balones de 45 kg", Destination = "Restaurante El Mar", ScheduledTime = new DateTime(2026, 6, 16, 14, 50, 0), Status = EDeliveryStatus.OnRoute },
            new { Id = 5, DriverId = 105, ResponsibleId = 5, VehicleId = 5, ItemCount = 5, Cargo = "5 balones", Destination = "Calle Lima 88", ScheduledTime = new DateTime(2026, 6, 16, 11, 15, 0), Status = EDeliveryStatus.Delivered, DeliveredAt = "11:15" },
            new { Id = 6, DriverId = 106, ResponsibleId = 6, VehicleId = 6, ItemCount = 1, Cargo = "1 balón", Destination = "Av. Grau 200", ScheduledTime = new DateTime(2026, 6, 16, 16, 0, 0), Status = EDeliveryStatus.NotDelivered });

        builder.Entity<DriverLocation>().ToTable("driver_locations");
        builder.Entity<DriverLocation>().HasKey(l => l.Id);
        builder.Entity<DriverLocation>().Property(l => l.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<DriverLocation>().Property(l => l.DriverId).IsRequired();
        builder.Entity<DriverLocation>().Property(l => l.Latitude).IsRequired();
        builder.Entity<DriverLocation>().Property(l => l.Longitude).IsRequired();
        builder.Entity<DriverLocation>().Property(l => l.LastUpdated).IsRequired();

        builder.Entity<DriverLocation>()
            .HasOne(l => l.Delivery)
            .WithMany()
            .HasForeignKey(l => l.DeliveryId);

        builder.Entity<DriverLocation>().HasData(
            new { Id = 1, DeliveryId = 1, DriverId = 101, Latitude = -12.0464, Longitude = -77.0428, LastUpdated = DateTime.UtcNow },
            new { Id = 2, DeliveryId = 2, DriverId = 102, Latitude = -12.0600, Longitude = -77.0375, LastUpdated = DateTime.UtcNow },
            new { Id = 3, DeliveryId = 4, DriverId = 104, Latitude = -12.0550, Longitude = -77.0400, LastUpdated = DateTime.UtcNow });
    }
}
