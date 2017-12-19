using System;
using Contracts.Commands;
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
                    instance.Send(new RestoreOrdersCommand()).GetAwaiter().GetResult();
                }
            }

            instance.Stop().GetAwaiter().GetResult();
        }
    }
}