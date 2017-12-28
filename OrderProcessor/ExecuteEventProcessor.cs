using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        private readonly EventProvider eventContext;
        private readonly IMapper mapper;

        private readonly Dictionary<Type, Func<object, IMessageHandlerContext, Task>> funcDictionary =
            new Dictionary<Type, Func<object, IMessageHandlerContext, Task>>();

        private readonly List<MethodInfo> methodInfos = new List<MethodInfo>();

        public IMessageHandlerContext MessageHandlerContext { get; set; }

        public ExecuteEventProcessor(OrderContext orderContext, EventProvider eventContext, IMapper mapper)
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

                if (MessageHandlerContext == null)
                    throw new ArgumentNullException(nameof(MessageHandlerContext));

                var handleEventMethod = typeof(THandler).GetMethod("Handle", new[] { data.GetType(), MessageHandlerContext.GetType() });

                if (handleEventMethod == null)
                    throw new Exception("Handle method cannot be found");

                dynamic task = handleEventMethod.Invoke(handlerInstance, new[] { data, MessageHandlerContext });

                await task;
            }
        }

        public async Task PerformEventsByStreamWithExpression<THandler>(string streamName, THandler handler)
        {
            var events = await eventContext.ReadStreamEventsForwardAsync(streamName);

            foreach (var @event in events)
            {
                var message = mapper.Map(@event, @event.Data, @event.GetType(), @event.Data.GetType());

                if (MessageHandlerContext == null)
                    throw new ArgumentNullException(nameof(MessageHandlerContext));

                var func = funcDictionary.FirstOrDefault(o => o.Key == message.GetType());

                if (func.Value == null)
                {
                    var expression = CreateExpression(handler, message);

                    var result = expression.Compile();

                    funcDictionary.Add(message.GetType(), result);

                    await result(message, MessageHandlerContext);
                }
                else
                {
                    await func.Value(message, MessageHandlerContext);
                }
            }
        }

        public Func<TEvent, IMessageHandlerContext, Task> GetExpressionFunc<TEvent, THandler>(TEvent data,
            IMessageHandlerContext context, THandler handler)
        {
            var message = Expression.Parameter(typeof(TEvent), "message");
            var handleContext = Expression.Parameter(typeof(IMessageHandlerContext), "context");

            var castedMessage = Expression.Convert(message, data.GetType());

            //var call = Expression.Call(, handleMethod, Expression.Convert(message, data.GetType()), handleContext);

            var call = Expression.Call(handler.GetType(), "Handle",
                new[] { data.GetType(), typeof(IMessageHandlerContext) },
                castedMessage, handleContext);

            var lambda = Expression.Lambda<Func<TEvent, IMessageHandlerContext, Task>>(call, message, handleContext);

            return lambda.Compile();
        }

        public Expression<Func<TEvent, IMessageHandlerContext, Task>> CreateExpression<THandler, TEvent>(
            THandler handler, TEvent data)
        {
            var messageType = Expression.Parameter(typeof(TEvent), "message");
            var message = Expression.Convert((messageType), data.GetType());
            var context = Expression.Parameter(typeof(IMessageHandlerContext), "context");

            var handlerConstant = Expression.Constant(handler);

            var handleMethod =
                methodInfos.FirstOrDefault(o =>
                    o.Name == "Handle" && o.DeclaringType == handler.GetType() &&
                    o.GetParameters().Select(p => p.ParameterType).Contains(data.GetType()));

            if (handleMethod == null)
            {
                handleMethod = handler.GetType()
                    .GetMethod("Handle", new[] { data.GetType(), typeof(IMessageHandlerContext) });

                if (handleMethod == null)
                    throw new InvalidOperationException();

                methodInfos.Add(handleMethod);
            }

            var call = Expression.Call(handlerConstant, handleMethod, message, context);

            return Expression.Lambda<Func<TEvent, IMessageHandlerContext, Task>>(call, messageType, context);
        }
    }
}