using System;

namespace Contracts.Commands
{
    public class CancelOrderCommand : BaseCommand
    {
        public Guid OrderId { get; set; }

        public CancelOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}