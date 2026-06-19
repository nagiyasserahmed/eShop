using eShop.Application.Interfaces;
using eShop.Infrastructure.Caching;
using eShop.Infrastructure.Messaging;
using eShop.Infrastructure.Messaging.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace eShop.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration =
                    configuration.GetConnectionString("RedisConnection");

                options.InstanceName = "eShop_";
            });

            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(
                    configuration.GetConnectionString("RedisConnection")!));

            services.AddScoped<ICacheService, CacheService>();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<OrderCreatedConsumer>();
                x.AddConsumer<StockReservedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(
                        configuration["RabbitMQ:Host"],
                        h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });

                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddScoped<ICacheService, CacheService>();

            services.AddScoped<IMessageBus,
                RabbitMqMessageBus>();
            
            return services;
        }
    }
}
