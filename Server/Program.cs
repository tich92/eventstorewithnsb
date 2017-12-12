using System;

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
            }

            instance.Stop().GetAwaiter().GetResult();
        }
    }
}