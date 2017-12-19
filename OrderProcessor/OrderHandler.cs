using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Events;
using NServiceBus;
using OrderProcessor.Commands;
using OrderProcessor.Data;
using OrderProcessor.Models;
using Order = OrderProcessor.Models.Order;

namespace OrderProcessor
{
    public class OrderHandler : IHandleMessages<CreatedOrderEvent>,
        IHandleMessages<CreatedOrderItemEvent>,
        IHandleMessages<PlacedOrderEvent>,
        IHandleMessages<CancelOrderEvent>
    {
        private readonly IMapper mapper;
        private readonly OrderContext orderContext;

        public OrderHandler(IMapper mapper, OrderContext orderContext)
        {
            this.mapper = mapper;
            this.orderContext = orderContext;
        }

        public async Task Handle(CreatedOrderEvent message, IMessageHandlerContext context)
        {
            var model = mapper.Map<Order>(message);

            orderContext.Orders.Add(model);
            await orderContext.SaveChangesAsync();
        }

        public async Task Handle(CreatedOrderItemEvent message, IMessageHandlerContext context)
        {
            var model = mapper.Map<OrderItem>(message);

            var existItem = orderContext.OrderItems.FirstOrDefault(o => o.Id == message.Id);

            if (existItem == null)
            {
                orderContext.OrderItems.Add(model);
                
                await orderContext.SaveChangesAsync();
            }

            await context.SendLocal(new CalculateOrderCommand(message.OrderId));
        }

        public async Task Handle(PlacedOrderEvent message, IMessageHandlerContext context)
        {
            var order = orderContext.Orders.FirstOrDefault(o => o.Id == message.OrderId);

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            order.Place();

            orderContext.Orders.Attach(order);

            await orderContext.SaveChangesAsync();
        }

        public async Task Handle(CancelOrderEvent message, IMessageHandlerContext context)
        {
            var order = orderContext.Orders.FirstOrDefault(o => o.Id == message.OrderId);

            if(order == null)
                throw new ArgumentNullException(nameof(order));

            order.Cancel();

            orderContext.Orders.Attach(order);

            await orderContext.SaveChangesAsync();
        }
    }
}