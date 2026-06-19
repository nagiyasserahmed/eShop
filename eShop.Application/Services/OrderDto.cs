using System;
using System.Collections.Generic;
using System.Text;
using eShop.Application.DTOs;

namespace eShop.Application.Services
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public ICollection<OrderItemDto> Items { get; set; } = null!;
    }
}
