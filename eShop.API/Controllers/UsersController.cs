using eShop.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IeShopDbContext eShopDbContext) : ControllerBase
    {
        [HttpGet]
        public Task<IActionResult> GetUsers()
        {
            var users = eShopDbContext.Users.ToList();
            return Task.FromResult<IActionResult>(Ok(users));
        }


        [HttpPost]
        public async Task<IActionResult> CreateUser(string name)
        {
            var user = new Domain.Models.User { Name = name };
            eShopDbContext.Users.Add(user);
            await eShopDbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }
    }
}
