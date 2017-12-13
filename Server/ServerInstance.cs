using System.Threading.Tasks;
using Contracts.Commands;
using NServiceBus;
using EventStoreContext;

namespace Server
{
    public class ServerInstance
    {
        public static async Task<IEndpointInstance> Initialize()
        {
            var endpointConfiguration = new EndpointConfiguration("Server");

            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();

            var routing = transport.Routing();

            routing.RouteToEndpoint(typeof(CheckOutOrderCommand).Assembly, "OrderProcessor");
            
            endpointConfiguration.UsePersistence<InMemoryPersistence>();

            endpointConfiguration.UseSerialization<JsonSerializer>();

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(type => type.Namespace != null && type.Namespace.Contains("Commands"));
            conventions.DefiningEventsAs(type => type.Namespace != null && type.Namespace.Contains("Events"));

            var mapping = new Mapping();

            var eventStore = new EventContext();

            endpointConfiguration.RegisterComponents(reg =>
            {
                reg.ConfigureComponent(() => mapping.Mapper, DependencyLifecycle.SingleInstance);
                reg.ConfigureComponent(() => eventStore, DependencyLifecycle.SingleInstance);
            });

            var endpointInstance = await Endpoint.Start(endpointConfiguration);
            return endpointInstance;
        }
    }
}