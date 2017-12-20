namespace OrderProcessor
{
    using EventStoreContext.Projections;
    using EventStoreContext;
    using NServiceBus;
    using Data;

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

            var projectionContext = new ProjectionContext();

            endpointConfiguration.RegisterComponents(reg =>
            {
                reg.ConfigureComponent(() => new OrderContext("OrderContext"), DependencyLifecycle.InstancePerCall);
                reg.ConfigureComponent(() => mapper.Mapper, DependencyLifecycle.SingleInstance);
                reg.ConfigureComponent(() => new EventContext(), DependencyLifecycle.SingleInstance);
                reg.ConfigureComponent(() => projectionContext, DependencyLifecycle.SingleInstance);

                reg.ConfigureComponent<ExecuteEventProcessor>(DependencyLifecycle.SingleInstance);
            });

            var projectionProvider = new CustomProjectionProvider(projectionContext);
            projectionProvider.RunProjections().GetAwaiter().GetResult();

            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");
        }
    }
}