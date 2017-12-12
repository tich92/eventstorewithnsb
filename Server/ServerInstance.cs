using System.Threading.Tasks;
using NServiceBus;

namespace Server
{
    public class ServerInstance
    {
        public static async Task<IEndpointInstance> Initialize()
        {
            var endpointConfiguration = new EndpointConfiguration("Server");

            endpointConfiguration.UseTransport<RabbitMQTransport>();
            endpointConfiguration.UsePersistence<InMemoryPersistence>();

            endpointConfiguration.UseSerialization<JsonSerializer>();

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(type => type.Namespace != null && type.Namespace.Contains("Commands"));
            conventions.DefiningEventsAs(type => type.Namespace != null && type.Namespace.Contains("Events"));

            var mapping = new Mapping();

            endpointConfiguration.RegisterComponents(reg =>
            {
                reg.ConfigureComponent(() => mapping.Mapper, DependencyLifecycle.SingleInstance);
            });

            var endpointInstance = await Endpoint.Start(endpointConfiguration);
            return endpointInstance;
        }
    }
}