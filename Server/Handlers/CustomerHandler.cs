using System.Threading.Tasks;
using AutoMapper;
using Contracts.Commands;
using Contracts.Events;
using EventStoreContext;
using NServiceBus;

namespace Server.Handlers
{
    public class CustomerHandler : IHandleMessages<CreateCustomerCommand>
    {
        private readonly IMapper mapper;
        private readonly EventProvider eventContext;

        private const string DefaultStreamName = "Customer";

        public CustomerHandler(IMapper mapper, EventProvider eventContext)
        {
            this.mapper = mapper;
            this.eventContext = eventContext;
        }

        public async Task Handle(CreateCustomerCommand message, IMessageHandlerContext context)
        {
            var @event = mapper.Map<CreateCustomerEvent>(message);

            var result = await eventContext.AddAsync($"{DefaultStreamName} {message.Id}", @event);

            @event.NextExpectedVersion = result.NextExpectedVersion;
            @event.LogPosition = result.LogPosition;

            await context.Publish(@event);
        }
    }
}