using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NServiceBus;

namespace EventProcessor.Caching
{
    public class FuncCacheProvider
    {
        private readonly ConcurrentDictionary<Type, Func<object, IMessageHandlerContext, Task>> funcs;

        public FuncCacheProvider()
        {
            funcs = new ConcurrentDictionary<Type, Func<object, IMessageHandlerContext, Task>>();
        }

        public Expression<Func<TEvent, IMessageHandlerContext, Task>> CreateFunc<THandler, TEvent>(THandler handler, TEvent @event)
        {
            var messageParameter = Expression.Parameter(typeof(TEvent), "message");
            var messageConverted = Expression.Convert(messageParameter, @event.GetType());

            var context = Expression.Parameter(typeof(IMessageHandlerContext), "context");

            var handlerConstant = Expression.Constant(handler);

            var handlerType = handler.GetType();

            var handleMethod = handlerType
                .GetMethod("Handle", new[] {@event.GetType(), typeof(IMessageHandlerContext)});

            if(handleMethod == null)
                throw new InvalidOperationException($"Cannot find Handle method in {handlerType.Name}");

            var methodCall = Expression.Call(handlerConstant, handleMethod, messageConverted, context);

            return Expression.Lambda<Func<TEvent, IMessageHandlerContext, Task>>(methodCall, messageParameter, context);
        }
    }
}