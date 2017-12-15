using System;

namespace AnotherClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var instance = ClientInstance.Initialize().GetAwaiter().GetResult();

            while (true)
            {
                var line = Console.ReadLine();

                if(line == null)
                    continue;
                
                if(line == "exit")
                    break;

                if (line == "new")
                {
                    
                }
            }

            instance.Stop().GetAwaiter().GetResult();
        }
    }
}
