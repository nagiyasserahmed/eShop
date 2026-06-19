using System;
using System.Collections.Generic;
using System.Text;
using eShop.Application.Interfaces;

namespace eShop.Application.Services
{
    public class PaymentService(IMessageBus messageBus) : IPaymentService
    {
    }
}
