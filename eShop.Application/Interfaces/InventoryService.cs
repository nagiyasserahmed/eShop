using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Application.Interfaces
{
    public class InventoryService(IMessageBus messageBus) : IInventoryService
    {
    }
}
