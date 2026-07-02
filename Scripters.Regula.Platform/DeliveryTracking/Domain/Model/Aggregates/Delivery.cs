using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Entities;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.ValueObjects;
using Scripters.Regula.Platform.Shared.Domain.Model.Entities;

namespace Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Aggregates;

public class Delivery : IAuditableEntity
{
    public int Id { get; private set; }

    public int DriverId { get; private set; }

    public int ResponsibleId { get; private set; }

    public DeliveryResponsible Responsible { get; private set; } = null!;

    public int VehicleId { get; private set; }

    public DeliveryVehicle Vehicle { get; private set; } = null!;

    public EDeliveryStatus Status { get; private set; }

    public int ItemCount { get; private set; }

    public string Cargo { get; private set; } = string.Empty;

    public string Destination { get; private set; } = string.Empty;

    public DateTime ScheduledTime { get; private set; }

    public string? DeliveredAt { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    protected Delivery()
    {
    }

    public Delivery(int driverId, int responsibleId, int vehicleId, int itemCount, string cargo, string destination, DateTime scheduledTime)
    {
        DriverId = driverId;
        ResponsibleId = responsibleId;
        VehicleId = vehicleId;
        ItemCount = itemCount;
        Cargo = cargo;
        Destination = destination;
        ScheduledTime = scheduledTime;
        Status = EDeliveryStatus.Pending;
    }

    public bool CanTransitionTo(EDeliveryStatus newStatus)
    {
        return (Status, newStatus) switch
        {
            (EDeliveryStatus.Pending, EDeliveryStatus.OnRoute) => true,
            (EDeliveryStatus.OnRoute, EDeliveryStatus.Delivered) => true,
            (EDeliveryStatus.OnRoute, EDeliveryStatus.NotDelivered) => true,
            _ => false
        };
    }

    public void UpdateStatus(EDeliveryStatus newStatus, string? deliveredAt)
    {
        Status = newStatus;
        if (newStatus == EDeliveryStatus.Delivered)
            DeliveredAt = deliveredAt;
    }
}
