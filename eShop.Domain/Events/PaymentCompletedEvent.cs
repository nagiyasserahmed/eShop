namespace eShop.Domain.Events;

public record PaymentCompletedEvent(
    Guid OrderId
);