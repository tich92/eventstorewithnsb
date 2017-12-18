using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Commands;
using Contracts.Events;
using EventStoreContext;
using Newtonsoft.Json;
using NServiceBus;
using OrderProcessor.Commands;
using OrderProcessor.Data;

namespace OrderProcessor
{
    public class OrderCommandHandler : IHandleMessages<CalculateOrderCommand>,
        IHandleMessages<CheckOutOrderCommand>
    {
        private readonly EventContext eventContext;
        private readonly IMapper mapper;
        private readonly OrderContext orderContext;

        public OrderCommandHandler(IMapper mapper, OrderContext orderContext, EventContext eventContext)
        {
            this.eventContext = eventContext;
            this.mapper = mapper;
            this.orderContext = orderContext;
        }
        public async Task Handle(CalculateOrderCommand message, IMessageHandlerContext context)
        {
            var order = orderContext.Orders.FirstOrDefault(o => o.Id == message.OrderId);

            if(order == null)
                throw new ArgumentNullException(nameof(order));

            order.OrderItems = orderContext.OrderItems.Where(o => o.OrderId == message.OrderId).ToList();

            order.CalculateOrder();

            orderContext.Orders.Attach(order);
            orderContext.Entry(order).State = EntityState.Modified;

            await orderContext.SaveChangesAsync();
        }

        public async Task Handle(CheckOutOrderCommand message, IMessageHandlerContext context)
        {
            var eventsResult = await eventContext.ReadStreamEventsBackward($"Order {message.OrderId}");

            if (eventsResult.Any())
            {
                foreach (var @event in eventsResult)
                {
                    await context.Publish(@event);
                }
            }
        }
    }
}