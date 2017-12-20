using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EventStoreContext;
using NServiceBus;
using OrderProcessor.Data;
using OrderProcessor.Handlers;

namespace OrderProcessor
{
    public class ExecuteEventProcessor
    {
        private readonly OrderContext orderContext;
        private readonly EventContext eventContext;
        private readonly OrderHandler orderHandler;
        private readonly IMessageHandlerContext messageHandlerContext;
        private readonly IMapper mapper;

        public delegate object GenericInvoker(object target, params object[] arguments);

        public ExecuteEventProcessor(OrderContext orderContext, EventContext eventContext, IMapper mapper,
            OrderHandler orderHandler, IMessageHandlerContext messageHandlerContext)
        {
            this.orderContext = orderContext;
            this.eventContext = eventContext;
            this.mapper = mapper;
            this.orderHandler = orderHandler;
            this.messageHandlerContext = messageHandlerContext;
        }

        public async Task DropDataAsync()
        {
            await orderContext.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[Orders]");
            await orderContext.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[OrderItems]");
        }

        public async Task<IEnumerable<string>> ExecuteDataFromStore()
        {
            return await eventContext.GetSrteamListAsync();
        }

        /// <summary>
        /// This variant of performing events more abstract and fast, but require - one RabbitMQ instance per service instance
        /// </summary>
        /// <param name="streamName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Obsolete("This approach is not correct, because all incoming events perform async")]
        public async Task PerformEventsByStreamAsync(string streamName, IMessageHandlerContext context)
        {
            var events = await eventContext.ReadStreamEventsForwardAsync(streamName);

            foreach (var @event in events)
            {
                var data = mapper.Map(@event, @event.Data, @event.GetType(), @event.Data.GetType());

                await context.Publish(data);
            }
        }

        public async Task PerformEventsByStreamAsync(string streamName)
        {
            var events = await eventContext.ReadStreamEventsForwardAsync(streamName);
            
            foreach (var @event in events)
            {
                var data = mapper.Map(@event, @event.Data, @event.GetType(), @event.Data.GetType());

                var handleEventMethod = orderHandler.GetType().GetMethod("Handle", new [] {data.GetType(), messageHandlerContext.GetType() });

                if (handleEventMethod == null)
                    throw new Exception("Handle method cannot be found");

                dynamic task = handleEventMethod.Invoke(orderHandler, new [] {data, messageHandlerContext});

                await task;
            }
        }
    }
}