using eShop.Application.Interfaces;
using eShop.Domain.Events;
using MassTransit;


namespace eShop.Infrastructure.Messaging.Consumers
{
    public class StockReservedConsumer(
        IMessageBus messageBus)
                : IConsumer<StockReservedEvent>
    {

        public async Task Consume(
            ConsumeContext<StockReservedEvent> context)
        {
            Console.WriteLine("Payment Completed");
        }
    }
}
