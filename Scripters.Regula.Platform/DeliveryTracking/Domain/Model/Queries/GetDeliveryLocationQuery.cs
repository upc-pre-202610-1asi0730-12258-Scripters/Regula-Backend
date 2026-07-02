using Scripters.Regula.Platform.DeliveryTracking.Domain.Model.ValueObjects;

namespace Scripters.Regula.Platform.DeliveryTracking.Domain.Model.Queries;

public record GetDeliveryLocationQuery(int DeliveryId);

public record GetDeliveriesByDateAndStatusQuery(DateOnly Date, EDeliveryStatus Status);

public record GetDeliveryByIdQuery(int DeliveryId);

public record GetAllDistributorDeliveriesQuery;

public record GetAllDistributorDeliverersQuery;
