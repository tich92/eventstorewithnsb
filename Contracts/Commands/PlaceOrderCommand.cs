using System;

namespace Contracts.Commands
{
    public class PlaceOrderCommand : BaseCommand
    {
        public PlaceOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; set; }
    }
}