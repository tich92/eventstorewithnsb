using System;
using EventProcessor.Caching;

namespace TestConsoleForEventProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var typeProvider = new TypeProvider();

            var events = typeProvider.GetReferencedTypes(o =>
                o.IsClass && o.Namespace.Contains("Contracts.Events") && o.Name.EndsWith("Event"));

            foreach (var type in events)
            {
                Console.WriteLine(type.FullName);
            }

            var handlers = typeProvider.GetReferencedTypes(o =>
                o.IsClass && o.Namespace.Contains("OrderProcessor") && o.Name.Contains("Handler"));

            Console.ReadLine();
        }
    }
}
