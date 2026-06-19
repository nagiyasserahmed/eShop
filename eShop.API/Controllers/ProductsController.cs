using Microsoft.AspNetCore.Mvc;
using eShop.Application.DTOs;
using eShop.Application.Interfaces;

namespace eShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductsService productsService) : ControllerBase
    {
        [HttpPost]
        public IActionResult Create(CreateProductDto createProductDto)
        {
            var createdProduct = productsService.Create(createProductDto).Result;
            return CreatedAtAction(nameof(Create), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = productsService.GetAll().Result;
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = productsService.GetById(id).Result;
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
    }
}
