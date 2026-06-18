using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Domain.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double Stock { get; set; }

        // Navigation property
        public ICollection<OrderItem> OrderItems { get; set; } = null!;
    }
}
