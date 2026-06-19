using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Application.Interfaces
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(T message);
    }
}
