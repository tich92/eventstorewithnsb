using System.Threading.Tasks;
using Contracts;
using NServiceBus;

namespace Client
{
    public class ClientInstance
    {
        public static async Task<IEndpointInstance> Initialize()
        {
            var endpointConfiguration = new EndpointConfiguration("Client");

            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();

            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(BaseCommand).Assembly, "Server");

            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            endpointConfiguration.UseSerialization<JsonSerializer>();

            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            var conventions = endpointConfiguration.Conventions();

            conventions.DefiningCommandsAs(type => type.Namespace != null && type.Namespace.Contains("Commands"));

            var instance = await Endpoint.Start(endpointConfiguration);

            return instance;
        }
    }
}