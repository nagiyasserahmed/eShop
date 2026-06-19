using eShop.Application.Interfaces;
using eShop.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IProductsService, ProductsService>();
            services.AddScoped<IOrderService, OrderService>();
            return services; 
        }
    }
}
