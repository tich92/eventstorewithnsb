using System;
using Contracts.Commands;
using EventStoreContext;
using NServiceBus;

namespace Server
{
    internal class Program
    {
        private static void Main()
        {
            Console.Title = "Server";

            var instance = ServerInstance.Initialize().GetAwaiter().GetResult();

            Console.WriteLine("Start workinkg server . . . ");
            Console.WriteLine("Waiting messages . . . ");

            var projectionContext = new ProjectionContext();

            var customerProducer = new CustomerProducer(projectionContext, instance);

            while (true)
            {
                var line = Console.ReadLine();

                if (line == null)
                    continue;

                if (line == "exit")
                    break;

                if (line.Contains("check-out"))
                {
                    var commandItems = line.Split(' ');

                    var orderId = Guid.Parse(commandItems[1]);

                    instance.Send(new CheckOutOrderCommand
                    {
                        OrderId = orderId
                    }).GetAwaiter().GetResult();
                }
                else if (line == "order-processor-restore")
                {
                    instance.Send(new RestoreOrderProcessorCommand()).GetAwaiter().GetResult();
                }
                else if (line == "propagate-customers")
                {
                    customerProducer.PropagateCustomersFromStoreAsync().GetAwaiter().GetResult();
                }
            }

            instance.Stop().GetAwaiter().GetResult();
        }
    }
}