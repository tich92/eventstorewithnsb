using System.Threading.Tasks;
using Contracts.Commands;
using EventStoreContext;
using NServiceBus;

namespace OrderProcessor.Handlers
{
    public class RestoreCommandHandler : IHandleMessages<RestoreOrderProcessorCommand>
    {
        private readonly OrderHandler orderHandler;
        private readonly CustomerHandler customerHandler;
        private readonly ExecuteEventProcessor executeEventProcessor;
        private readonly ProjectionProvider projectionContext;
        private readonly EventProvider eventContext;

        private static object _lockObject = new object();

        public RestoreCommandHandler(OrderHandler orderHandler, ExecuteEventProcessor executeEventProcessor,
            ProjectionProvider projectionContext, EventProvider eventContext, CustomerHandler customerHandler)
        {
            this.orderHandler = orderHandler;
            this.executeEventProcessor = executeEventProcessor;
            this.projectionContext = projectionContext;
            this.eventContext = eventContext;
            this.customerHandler = customerHandler;
        }

        public Task Handle(RestoreOrderProcessorCommand message, IMessageHandlerContext context)
        {
            lock (_lockObject)
            {
                executeEventProcessor.MessageHandlerContext = context;

                executeEventProcessor.DropDataAsync().GetAwaiter().GetResult();

                RestoreCustomersAsync().GetAwaiter().GetResult();
                RestoreOrdersAsync().GetAwaiter().GetResult();
            }

            return Task.CompletedTask;
        }

        private async Task RestoreOrdersAsync()
        {
            var projectionResult = await projectionContext.GetListOfOrderStreamsAsync();

            foreach (var stream in projectionResult.Items)
            {
                await executeEventProcessor.PerformEventsByStreamAsync(stream, orderHandler);
            }
        }

        private async Task RestoreCustomersAsync()
        {
            var projectionResult = await projectionContext.GetListOfCustomerStreamsAsync();

            foreach (var stream in projectionResult.Items)
            {
                await executeEventProcessor.PerformEventsByStreamAsync(stream, customerHandler);
            }
        }
    }
}