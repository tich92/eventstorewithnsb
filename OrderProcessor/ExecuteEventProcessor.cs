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

        public async Task PerformEventsByStreamAsync(string streamName)
        {
            var events = await eventContext.ReadStreamEventsForwardAsync(streamName);
            
            foreach (var @event in events)
            {
                var data = mapper.Map(@event, @event.Data, @event.GetType(), @event.Data.GetType());

                var handleEventMethod = orderHandler.GetType().GetMethod(nameof(OrderHandler.Handle), new [] {data.GetType(), messageHandlerContext.GetType() });

                if (handleEventMethod == null)
                    throw new Exception("Handle method cannot be found");

                handleEventMethod.Invoke(orderHandler, new [] {data, messageHandlerContext});
            }
        }
    }
}