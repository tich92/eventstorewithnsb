using System.Threading.Tasks;
using AutoMapper;
using Contracts.Commands;
using Contracts.Events;
using NServiceBus;

namespace Server
{
    public class OrderHandler : IHandleMessages<CreateOrderCommand>,
        IHandleMessages<AddOrderItemCommand>,
        IHandleMessages<PlaceOrderCommand>
    {
        private IMapper _mapper;

        public OrderHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task Handle(CreateOrderCommand message, IMessageHandlerContext context)
        {
            var @event = _mapper.Map<ICreatedOrderEvent>(message);

            await context.Publish(@event);
        }

        public async Task Handle(AddOrderItemCommand message, IMessageHandlerContext context)
        {
            var @event = _mapper.Map<ICreatedOrderItemEvent>(message);

            await context.Publish(@event);
        }

        public async Task Handle(PlaceOrderCommand message, IMessageHandlerContext context)
        {
            var @event = _mapper.Map<IPlacedOrderEvent>(message);

            await context.Publish(@event);
        }
    }
}