using eShop.Application.Interfaces;
using MassTransit;

namespace eShop.Infrastructure.Messaging
{
    public class RabbitMqMessageBus : IMessageBus
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public RabbitMqMessageBus(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync<T>(T message)
        {
            await _publishEndpoint.Publish(message!);
        }
    }
}
