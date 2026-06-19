using eShop.Application.Interfaces;
using eShop.Domain.Events;
using eShop.Domain.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace eShop.Infrastructure.Messaging.Consumers
{
    public class OrderCreatedConsumer(
        IMessageBus messageBus,
        IeShopDbContext eShopDbContext)
        : IConsumer<OrderCreatedEvent>
    {
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            using var transaction = await eShopDbContext.BeginTransactionAsync();

            try
            {
                var productIds = context.Message.Items
                    .Select(i => i.ProductId)
                    .Distinct()
                    .ToList();

                var products = await eShopDbContext.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id);

                var order = new Order
                {
                    Id = context.Message.OrderId,
                    UserId = context.Message.UserId,
                    Items = []
                };

                foreach (var item in context.Message.Items)
                {
                    if (!products.TryGetValue(item.ProductId, out var product))
                        throw new Exception($"Product {item.ProductId} not found.");

                    if (product.Stock < item.Quantity)
                        throw new Exception(
                            $"Insufficient stock for product {product.Name}");

                    product.Stock -= item.Quantity;

                    order.Items.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    });
                }

                eShopDbContext.Orders.Add(order);

                await eShopDbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                await messageBus.PublishAsync(
                    new StockReservedEvent(order.Id));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();

                throw new Exception(
                    "Stock was modified by another transaction. Retry the operation.",
                    ex);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}