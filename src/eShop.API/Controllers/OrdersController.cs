using eShop.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using eShop.Domain.Models;

namespace eShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(eShopDbContext eShopDbContext) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetOrders()
        {
            var orders = eShopDbContext.Orders.ToList();
            return Ok(orders);
        }

        [HttpPost]
        public IActionResult CreateOrder(Order order)
        {
            IEnumerable<OrderItem> orderItems = order.Items;

            eShopDbContext.Products.Where(p => orderItems.Any(oi => oi.ProductId == p.Id)).ToList().ForEach(product =>
            {
                var orderItem = orderItems.First(oi => oi.ProductId == product.Id);

                if (product.Stock < orderItem.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for product {product.Name}");
                }
            });

            eShopDbContext.Orders.Add(order);

            eShopDbContext.SaveChanges();

            return CreatedAtAction(nameof(GetOrders), new { id = order.Id }, order);
        }
    }
}
