using eShop.Domain.Models;
using eShop.Application.Interfaces;
namespace eShop.Application.Services
{
    public class ProductsService(IeShopDbContext eShopDbContext)
    {
        public Product Create(Product product)
        {
            eShopDbContext.Products.Add(product);
            eShopDbContext.SaveChangesAsync();

            return product;
        }
    }
}
