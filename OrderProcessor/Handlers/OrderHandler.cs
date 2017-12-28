using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Events;
using EventStoreContext;
using NServiceBus;
using NServiceBus.Logging;
using OrderProcessor.Commands;
using OrderProcessor.Data;
using OrderProcessor.Models;
using Order = OrderProcessor.Models.Order;

namespace OrderProcessor.Handlers
{
    public class OrderHandler : IHandleMessages<CreatedOrderEvent>,
        IHandleMessages<CreatedOrderItemEvent>,
        IHandleMessages<PlacedOrderEvent>,
        IHandleMessages<CancelOrderEvent>
    {
        private static ILog _log = LogManager.GetLogger<OrderHandler>();

        private readonly IMapper mapper;
        private readonly OrderContext orderContext;
        private readonly EventProvider eventContext;
        private readonly ProjectionProvider projectionContext;

        private static object _lockObject = new object();

        public OrderHandler(IMapper mapper, OrderContext orderContext, EventProvider eventContext, ProjectionProvider projectionContext)
        {
            this.mapper = mapper;
            this.orderContext = orderContext;
            this.eventContext = eventContext;
            this.projectionContext = projectionContext;
        }

        public async Task Handle(CreatedOrderEvent message, IMessageHandlerContext context)
        {
            try
            {
                _log.Info($"Handled new order with Id {message.Id}");

                var model = mapper.Map<Order>(message);

                var existOrder = orderContext.Orders.FirstOrDefault(o => o.Id == model.Id);

                if(existOrder != null)
                    return;

                orderContext.Orders.Add(model);
                await orderContext.SaveChangesAsync();

                _log.Info($"Handling new order with Id {message.Id} successful performed");
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
                throw;
            }
        }

        public async Task Handle(CreatedOrderItemEvent message, IMessageHandlerContext context)
        {
            _log.Info($"Handled new order item with Id {message.Id}");

            var model = mapper.Map<OrderItem>(message);

            var existItem = orderContext.OrderItems.FirstOrDefault(o => o.Id == message.Id);

            if (existItem == null)
            {
                orderContext.OrderItems.Add(model);
                
                await orderContext.SaveChangesAsync();
            }

            await context.SendLocal(new CalculateOrderCommand(message.OrderId));

            _log.Info($"Handling new order item with Id {message.Id} successful performed");
        }

        public async Task Handle(PlacedOrderEvent message, IMessageHandlerContext context)
        {
            _log.Info($"Handled place order event with Id {message.OrderId}");

            var order = orderContext.Orders.FirstOrDefault(o => o.Id == message.OrderId);

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            order.Place();

            orderContext.Orders.Attach(order);
            //orderContext.Entry(order).State = EntityState.Modified;

            await orderContext.SaveChangesAsync();

            _log.Info($"Handling place order with Id {message.OrderId} successful performed");
        }

        public async Task Handle(CancelOrderEvent message, IMessageHandlerContext context)
        {
            _log.Info($"Handled cancel order event with Id {message.OrderId}");

            var order = orderContext.Orders.FirstOrDefault(o => o.Id == message.OrderId);

            if(order == null)
                throw new ArgumentNullException(nameof(order));

            order.Cancel();

            orderContext.Orders.Attach(order);
            //orderContext.Entry(order).State = EntityState.Modified;

            await orderContext.SaveChangesAsync();

            _log.Info($"Handling cancel order with Id {message.OrderId} successful performed");
        }
    }
}