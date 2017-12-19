using OrderProcessor.Data;

namespace OrderProcessor
{
    using EventStoreContext;
    using NServiceBus;

    [EndpointName("OrderProcessor")]
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Client
    {
        public void Customize(EndpointConfiguration endpointConfiguration)
        {
            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();

            endpointConfiguration.UsePersistence<InMemoryPersistence>();

            var conventions = endpointConfiguration.Conventions();

            conventions.DefiningCommandsAs(type => type.Namespace != null && type.Namespace.Contains("Commands"));
            conventions.DefiningEventsAs(type => type.Namespace != null && type.Namespace.Contains("Events"));

            var mapper = new MappingConfig();

            endpointConfiguration.RegisterComponents(reg =>
            {
                reg.ConfigureComponent(() => new OrderContext("OrderContext"), DependencyLifecycle.InstancePerCall);
                reg.ConfigureComponent(() => mapper.Mapper, DependencyLifecycle.SingleInstance);
                reg.ConfigureComponent(() => new EventContext(), DependencyLifecycle.SingleInstance);
                reg.ConfigureComponent(() => new ProjectionContext(), DependencyLifecycle.SingleInstance);

            });

            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");
        }
    }
}