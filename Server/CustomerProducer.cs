using System;
using System.Threading.Tasks;
using Contracts.Events;
using EventStoreContext;
using NServiceBus;

namespace Server
{
    public class CustomerProducer
    {
        private readonly ProjectionContext projectionContext;
        private readonly IMessageSession messageSession;

        public CustomerProducer(ProjectionContext projectionContext, IMessageSession messageSession)
        {
            this.projectionContext = projectionContext;
            this.messageSession = messageSession;
        }

        public async Task PropagateCustomersFromStoreAsync()
        {
            var projectionResult = await projectionContext.GetListOfCustomerIdAsync();

            foreach (var id in projectionResult.Items)
            {
                var customerId = Guid.Parse(id);

                var command = new CreateCustomerEvent()
                {
                    Id = customerId,
                    FullName = id,
                    CreatedDate = DateTime.Now,
                    Email = $"{id}@test.com",
                    Phone = "+380631472536"
                };

                await messageSession.Publish(command);
            }
        }
    }
}