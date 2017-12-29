using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using NServiceBus;

namespace EventProcessor
{
    public class NServiceBusHandlerProvider
    {
        private static Type ContextType => typeof(IMessageHandlerContext);
        private static ParameterExpression Context => Expression.Parameter(ContextType, "context");

        public Expression<Func<object, IMessageHandlerContext, Task>> CreateExpression(Type messageType, MethodInfo handleMethodInfo)
        {
            var messageExpression = Expression.Parameter(messageType, "message");
            var convertedMessageExpression = Expression.Convert(messageExpression, messageType);

            var callExpression = Expression.Call(handleMethodInfo, convertedMessageExpression, Context);

            return Expression.Lambda<Func<object, IMessageHandlerContext, Task>>(callExpression, messageExpression, Context);
        }

        public Expression<Func<object, IMessageHandlerContext, Task>> CreateExpression(MethodInfo handleInfo)
        {
            var messageType =
                handleInfo.GetParameters().FirstOrDefault(p => p.Name.ToLower() == "message")?.ParameterType ??
                throw new InvalidOperationException();

            var messageExpression =
                Expression.Parameter(typeof(object), "message");

            var convertedMessageExpression = Expression.Convert(messageExpression, messageType);

            var handlerType = handleInfo.ReflectedType;

            if(handlerType == null)
                throw new InvalidOperationException();

            var handlerExpression = Expression.Parameter(handlerType, "Handler");

            ParameterInfo[] parameters = GetConstructorParameters(handlerType);

            var handlerInstance = Activator.CreateInstance(handlerType, new Type[]{});

            var handlerConstant = Expression.Constant(handlerInstance);
            var handlerConvertedConstant = Expression.Convert(handlerConstant, handlerType);

            //var callExpression = Expression.Call(handlerType, "Handle", new [] { messageType, ContextType}, convertedMessageExpression, Context);
            var callExpression = Expression.Call(handlerConvertedConstant, handleInfo, convertedMessageExpression, Context);

            return Expression.Lambda<Func<object, IMessageHandlerContext, Task>>(callExpression, messageExpression, Context);
        }

        public Func<object, IMessageHandlerContext, Task> CompileFunc(
            Expression<Func<object, IMessageHandlerContext, Task>> expression)
        {
            return expression.Compile();
        }

        private ParameterInfo[] GetConstructorParameters(Type type)
        {
            var ctors = type.GetConstructors();
            // Assuming class ObjectType has only one constructor:
            var ctor = ctors[0];

            var parameters = ctor.GetParameters();
            
            return parameters;
        }
    }
}
