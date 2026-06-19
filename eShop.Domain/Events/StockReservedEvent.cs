namespace eShop.Domain.Events;

public record StockReservedEvent(
    Guid OrderId
);