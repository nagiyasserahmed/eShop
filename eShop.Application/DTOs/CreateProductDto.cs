using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Application.DTOs
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double Stock { get; set; }
    }
}
