using System;
using System.Linq;
using EventStoreContext;

namespace Client
{
    class Program
    {
        static void Main()
        {
            var endpointInstance = ClientInstance.Initialize().GetAwaiter().GetResult();
            var persistence = new OrderPersistence();

            var projectionContext = new ProjectionContext();

            var orderProducer = new OrderProducer(endpointInstance, persistence);
            

            Console.WriteLine("Client initialized . . .");
            while (true)
            {
                var line = Console.ReadLine();

                if (line == null) continue;
                if (line == "exit" || line == "q" || line == "quit")
                {
                    break;
                }
                if (line == "new")
                {
                    orderProducer.CreateOrderAsync().GetAwaiter().GetResult();
                }
                else if (line.Contains("add-item"))
                {
                    var commandItems = line.Split(' ');

                    var orderKey = int.Parse(commandItems[1]);

                    var orderId = persistence.OrderDictionary.FirstOrDefault(o => o.Key == orderKey).Value;

                    var quantity = int.Parse(commandItems[2]);
                    var price = decimal.Parse(commandItems[3]);

                    orderProducer.CreateOrderItem(orderId, quantity, price).GetAwaiter().GetResult();
                }
                else if (line.Contains("place"))
                {
                    var commandItems = line.Split(' ');
                    var orderKey = int.Parse(commandItems[1]);

                    var orderId = persistence.OrderDictionary.FirstOrDefault(o => o.Key == orderKey).Value;

                    orderProducer.PlaceOrder(orderId).GetAwaiter().GetResult();
                }
                else if(line.Contains("cancel"))
                {
                    var commandItems = line.Split(' ');

                    var orderKey = int.Parse(commandItems[1]);

                    var orderId = persistence.OrderDictionary.FirstOrDefault(o => o.Key == orderKey).Value;

                    orderProducer.CancelOrder(orderId).GetAwaiter().GetResult();
                }
            }

            Console.WriteLine("Close client . . .");
            endpointInstance.Stop().GetAwaiter().GetResult();
        }
    }
}