namespace eShop.Domain.Events;

public record OrderCreatedEvent(
    Guid OrderId,
    int UserId,
    ICollection<OrderItemMessage> Items);


public record OrderItemMessage(
    int ProductId,
    int Quantity);