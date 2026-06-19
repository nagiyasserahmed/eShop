using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double Stock { get; set; }
    }
}
