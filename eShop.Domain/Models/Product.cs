using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eShop.Domain.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double Stock { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = default!;

        // Navigation property
        public ICollection<OrderItem> OrderItems { get; set; } = null!;
    }
}
