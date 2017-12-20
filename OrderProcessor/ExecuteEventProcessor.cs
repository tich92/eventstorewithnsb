using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EventStoreContext;
using NServiceBus;
using OrderProcessor.Data;

namespace OrderProcessor
{
    public class ExecuteEventProcessor
    {
        private readonly OrderContext orderContext;
        private readonly EventContext eventContext;
        private readonly IMapper mapper;

        public IMessageHandlerContext MessageHandlerContext { get; set; }
        
        public ExecuteEventProcessor(OrderContext orderContext, EventContext eventContext, IMapper mapper)
        {
            this.orderContext = orderContext;
            this.eventContext = eventContext;
            this.mapper = mapper;
        }

        public async Task DropDataAsync()
        {
            await orderContext.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[Customers]");
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

        public async Task PerformEventsByStreamAsync<THandler>(string streamName, THandler handlerInstance)
        {
            var events = await eventContext.ReadStreamEventsForwardAsync(streamName);
            
            foreach (var @event in events)
            {
                var data = mapper.Map(@event, @event.Data, @event.GetType(), @event.Data.GetType());
                
                if(MessageHandlerContext == null)
                    throw new ArgumentNullException(nameof(MessageHandlerContext));

                var handleEventMethod = typeof(THandler).GetMethod("Handle", new[] { data.GetType(), MessageHandlerContext.GetType() });

                if (handleEventMethod == null)
                    throw new Exception("Handle method cannot be found");
                
                dynamic task = handleEventMethod.Invoke(handlerInstance, new [] {data, MessageHandlerContext });

                await task;
            }
        }
    }
}