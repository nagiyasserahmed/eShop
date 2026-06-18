using eShop.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.EF
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddEF(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<eShopDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IeShopDbContext>(provider =>
            provider.GetRequiredService<eShopDbContext>());
         
            return services;
        }
    }
}
