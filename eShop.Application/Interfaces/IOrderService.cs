using eShop.Application.DTOs;
using eShop.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> Place(CreateOrderDto orderDto);
        Task<ICollection<OrderDto>> GetOrders();
        Task<OrderDto?> GetOrderById(Guid id);
    }
}
