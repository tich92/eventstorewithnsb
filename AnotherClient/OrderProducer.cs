using System.Threading.Tasks;
using NServiceBus;

namespace AnotherClient
{
    public class OrderProducer
    {
        private readonly IMessageSession messageSession;

        public OrderProducer(IMessageSession messageSession)
        {
            this.messageSession = messageSession;
        }

        public async Task CreateOrderAsync()
        {
            
        }
    }
}
