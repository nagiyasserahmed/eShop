using eShop.Application.DTOs;
using eShop.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IOrderService orderService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
           var orders = await orderService.GetOrders();
           return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await orderService.GetOrderById(id); 
            
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto createOrderDto)
        {
            var createdOrder = await orderService.Place(createOrderDto);

            return Accepted(createdOrder);
        }
    }
}