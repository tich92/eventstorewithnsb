using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Commands;
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
        IHandleMessages<CancelOrderEvent>,
        IHandleMessages<RestoreOrdersCommand>
    {
        private static ILog _log = LogManager.GetLogger<OrderHandler>();

        private readonly IMapper mapper;
        private readonly OrderContext orderContext;
        private readonly EventContext eventContext;
        private readonly ProjectionContext projectionContext;

        private static object _lockObject = new object();

        public OrderHandler(IMapper mapper, OrderContext orderContext, EventContext eventContext, ProjectionContext projectionContext)
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
                _log.Info($"Handle {nameof(message)}");

                var model = mapper.Map<Order>(message);

                var existOrder = orderContext.Orders.FirstOrDefault(o => o.Id == model.Id);

                if(existOrder != null)
                    return;

                orderContext.Orders.Add(model);
                await orderContext.SaveChangesAsync();

                _log.Info($"Perform {nameof(message)} successful");
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
                throw;
            }
        }

        public async Task Handle(CreatedOrderItemEvent message, IMessageHandlerContext context)
        {
            _log.Info($"Handle {nameof(message)}");

            var model = mapper.Map<OrderItem>(message);

            var existItem = orderContext.OrderItems.FirstOrDefault(o => o.Id == message.Id);

            if (existItem == null)
            {
                orderContext.OrderItems.Add(model);
                
                await orderContext.SaveChangesAsync();
            }

            await context.SendLocal(new CalculateOrderCommand(message.OrderId));

            _log.Info($"Perform {nameof(message)} successful");
        }

        public async Task Handle(PlacedOrderEvent message, IMessageHandlerContext context)
        {
            _log.Info($"Handle {nameof(message)}");

            var order = orderContext.Orders.FirstOrDefault(o => o.Id == message.OrderId);

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            order.Place();

            orderContext.Orders.Attach(order);
            orderContext.Entry(order).State = EntityState.Modified;

            await orderContext.SaveChangesAsync();

            _log.Info($"Perform {nameof(message)} successful");
        }

        public async Task Handle(CancelOrderEvent message, IMessageHandlerContext context)
        {
            _log.Info($"Handle {nameof(message)}");

            var order = orderContext.Orders.FirstOrDefault(o => o.Id == message.OrderId);

            if(order == null)
                throw new ArgumentNullException(nameof(order));

            order.Cancel();

            orderContext.Orders.Attach(order);
            orderContext.Entry(order).State = EntityState.Modified;

            await orderContext.SaveChangesAsync();

            _log.Info($"Perform {nameof(message)} successful");
        }

        public Task Handle(RestoreOrdersCommand message, IMessageHandlerContext context)
        {
            lock (_lockObject)
            {
                var restoreProcessor = new ExecuteEventProcessor(orderContext, eventContext, mapper);

                restoreProcessor.MessageHandlerContext = context;

                restoreProcessor.DropDataAsync().GetAwaiter().GetResult();

                var streams = projectionContext.GetListOfOrderStreamsAsync().GetAwaiter().GetResult();

                foreach (var stream in streams.Items)
                {
                    restoreProcessor.PerformEventsByStreamAsync(stream, this).GetAwaiter().GetResult();
                }

                return Task.CompletedTask;
            }
        }
    }
}