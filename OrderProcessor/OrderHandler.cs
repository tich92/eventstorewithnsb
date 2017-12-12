using System.Threading.Tasks;
using Contracts.Events;
using NServiceBus;

namespace OrderProcessor
{
    public class OrderHandler : IHandleMessages<ICreatedOrderEvent>,
        IHandleMessages<ICreatedOrderItemEvent>,
        IHandleMessages<IPlacedOrderEvent>
    {
        public async Task Handle(ICreatedOrderEvent message, IMessageHandlerContext context)
        {
            throw new System.NotImplementedException();
        }

        public async Task Handle(ICreatedOrderItemEvent message, IMessageHandlerContext context)
        {
            throw new System.NotImplementedException();
        }

        public async Task Handle(IPlacedOrderEvent message, IMessageHandlerContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}