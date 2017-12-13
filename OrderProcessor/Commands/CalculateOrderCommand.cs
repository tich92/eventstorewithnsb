using System;

namespace OrderProcessor.Commands
{
    public class CalculateOrderCommand
    {
        public Guid OrderId { get; set; }

        public CalculateOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}