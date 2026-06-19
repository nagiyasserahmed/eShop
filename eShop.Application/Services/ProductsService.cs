using eShop.Domain.Models;
using eShop.Application.Interfaces;
using eShop.Application.DTOs;
using Microsoft.EntityFrameworkCore;
namespace eShop.Application.Services
{
    public class ProductsService(IeShopDbContext eShopDbContext, ICacheService cacheService, IMessageBus messageBus) : IProductsService
    {
        public async Task<Product> Create(CreateProductDto createProductDto)
        {
            Product product = new Product
            {
                Name = createProductDto.Name,
                Price = createProductDto.Price,
                Stock = createProductDto.Stock,
            };

            eShopDbContext.Products.Add(product);

            await eShopDbContext.SaveChangesAsync();

            await cacheService.SetAsync($"product:{product.Id}:stock", product.Stock);

            return product;
        }

        public async Task<IEnumerable<ProductDto>> GetAll()
        {
            List<Product> products = await eShopDbContext.Products.AsNoTracking().ToListAsync<Product>();

            return [.. products.Select(p => Map(p))];
        }

        public async Task<ProductDto?> GetById(int id)
        {
            var cachedProduct = await cacheService.GetAsync<ProductDto>($"product_{id}");

            if (cachedProduct != null)
            {
                return new ProductDto
                {
                    Id = cachedProduct.Id,
                    Name = cachedProduct.Name,
                    Price = cachedProduct.Price,
                    Stock = cachedProduct.Stock
                };
            }

            var product = await eShopDbContext.Products.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return null;
            }

            var dto = Map(product);

            await cacheService.SetAsync($"product_{id}", dto, TimeSpan.FromMinutes(1));

            return Map(product);
        }

        private static ProductDto Map(Product p)
        {
            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock
            };
        }
    }
}
