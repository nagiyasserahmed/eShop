using eShop.Application.DTOs;
using eShop.Domain.Models;

namespace eShop.Application.Services
{
    public interface IProductsService
    {
        Task<Product> Create(CreateProductDto createProductDto);
        Task<IEnumerable<ProductDto>> GetAll();
        Task<ProductDto?> GetById(int id);
    }
}
