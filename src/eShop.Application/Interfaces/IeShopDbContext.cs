using eShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
namespace eShop.Application.Interfaces
{
    public interface IeShopDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Product> Products { get; }
        DbSet<Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}   
