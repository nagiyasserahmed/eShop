using eShop.Application.DTOs;
using eShop.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using eShop.Domain.Events;

namespace eShop.Application.Services
{
    public class OrderService(
        IeShopDbContext eShopDbContext,
        ICacheService cacheService,
        IMessageBus messageBus) : IOrderService
    {
        public async Task<OrderDto> Place(CreateOrderDto orderDto)
        {
            if (orderDto.Items == null || !orderDto.Items.Any())
                throw new Exception("Order must contain at least one item.");

            var orderId = Guid.NewGuid();
            var reservedItems = new List<string>();

            try
            {
                foreach (var item in orderDto.Items)
                {
                    var cacheKey = $"product:{item.ProductId}:stock";

                    // Step 1: Atomic check & decrement via Lua Script
                    bool isReserved = await cacheService.ReserveStockAsync(cacheKey, item.Quantity);

                    if (!isReserved)
                    {
                        throw new Exception($"Product {item.ProductId} is out of stock.");
                    }

                    // Track what we successfully reserved so we can roll back if a later item fails
                    reservedItems.Add(cacheKey);
                }
            }
            catch (Exception)
            {
                // Rollback previous allocations in this loop if a multi-item order fails midway
                foreach (var key in reservedItems)
                {
                    var item = orderDto.Items.First(x => $"product:{x.ProductId}:stock" == key);
                    await cacheService.IncrementStockAsync(key, item.Quantity);
                }
                throw;
            }

            await messageBus.PublishAsync(new OrderCreatedEvent(
                orderId,
                orderDto.UserId,
                [.. orderDto.Items.Select(x => new OrderItemMessage(x.ProductId, x.Quantity))]
            ));

            return new OrderDto
            {
                Id = orderId,
                UserId = orderDto.UserId,
                Items = [.. orderDto.Items.Select(x => new OrderItemDto { ProductId = x.ProductId, Quantity = x.Quantity })]
            };
        }

        public async Task<ICollection<OrderDto>> GetOrders()
        {
            var orders = await eShopDbContext.Orders
                .Include(o => o.Items)
                .ToListAsync();

            return [.. orders.Select(order => new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                TotalAmount = order.TotoalPrice,
                Items = [.. order.Items.Select(x => new OrderItemDto
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                })]
            })];
        }

        public async Task<OrderDto?> GetOrderById(Guid id)
        {
            var order = await eShopDbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return null;
            }
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                TotalAmount = order.TotoalPrice,
                Items = [.. order.Items.Select(x => new OrderItemDto
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                })]
            };
        }
    }
}