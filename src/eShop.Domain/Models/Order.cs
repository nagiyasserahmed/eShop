using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Domain.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotoalPrice { get; set; }
        public DateTime OrderDate { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<OrderItem> Items { get; set; } = null!;
    }
}
