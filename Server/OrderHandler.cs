using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Commands;
using Contracts.Events;
using EventStoreContext;
using NServiceBus;

namespace Server
{
    public class OrderHandler : IHandleMessages<CreateOrderCommand>,
        IHandleMessages<AddOrderItemCommand>,
        IHandleMessages<PlaceOrderCommand>
    {
        private IMapper mapper;
        private EventContext eventContext;

        public OrderHandler(IMapper mapper, EventContext eventContext)
        {
            this.mapper = mapper;
            this.eventContext = eventContext;
        }

        public async Task Handle(CreateOrderCommand message, IMessageHandlerContext context)
        {
            var @event = mapper.Map<CreatedOrderEvent>(message);

            await eventContext.Add($"Order {message.Id}", @event);

            await context.Publish(@event);
        }

        public async Task Handle(AddOrderItemCommand message, IMessageHandlerContext context)
        {
            var @event = mapper.Map<CreatedOrderItemEvent>(message);

            await eventContext.Add($"Order {message.OrderId}", @event);

            await context.Publish(@event);
        }

        public async Task Handle(PlaceOrderCommand message, IMessageHandlerContext context)
        {
            var @event = mapper.Map<PlacedOrderEvent>(message);

            await eventContext.Add($"Order {message.OrderId}", @event);

            await context.Publish(@event);
        }
    }
}