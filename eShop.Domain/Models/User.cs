using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Order> Orders { get; set; } = null!;
    }
}
