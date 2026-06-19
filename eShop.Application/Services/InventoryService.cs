using eShop.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Application.Services
{
    public class InventoryService(IMessageBus messageBus) : IInventoryService
    {
    }
}
