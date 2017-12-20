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
        private readonly ProjectionContext projectionContext;
        private readonly EventContext eventContext;

        public RestoreCommandHandler(OrderHandler orderHandler, ExecuteEventProcessor executeEventProcessor,
            ProjectionContext projectionContext, EventContext eventContext, CustomerHandler customerHandler)
        {
            this.orderHandler = orderHandler;
            this.executeEventProcessor = executeEventProcessor;
            this.projectionContext = projectionContext;
            this.eventContext = eventContext;
            this.customerHandler = customerHandler;
        }

        public async Task Handle(RestoreOrderProcessorCommand message, IMessageHandlerContext context)
        {
            executeEventProcessor.MessageHandlerContext = context;

            await executeEventProcessor.DropDataAsync();

            await RestoreCustomersAsync();
            await RestoreOrdersAsync();
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