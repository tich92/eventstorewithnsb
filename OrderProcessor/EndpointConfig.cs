namespace OrderProcessor
{
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

            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");
        }
    }
}