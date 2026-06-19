using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Application.DTOs
{
    public class CreateOrderDto
    {
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; } = 0;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public ICollection<OrderItemDto> Items { get; set; } = null!;
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
