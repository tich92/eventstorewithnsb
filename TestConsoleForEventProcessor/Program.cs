using System;
using System.Linq;
using System.Threading.Tasks;
using EventProcessor.Caching;
using EventProcessor.IoC;
using EventProcessor.IoC.ToBeDelete;

namespace TestConsoleForEventProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            IContainer container = new Container();

            TestIoC(container);
            
            var typeProvider = new TypeProvider();

            var events = typeProvider.GetReferencedTypes(o =>
                o.IsClass && o.Namespace.Contains("Contracts.Events") && o.Name.EndsWith("Event"));

            var handlers = typeProvider.GetReferencedTypes(h => h.Namespace.Contains("OrderProcessor") && h.Name.Contains("Handle"));

            foreach (var type in events)
            {
                Console.WriteLine(type.FullName);
            }
            
            foreach (var type in handlers)
            {
                var methodList = type.GetMethods().Where(o => o.Name == "Handle");

                //foreach (var method in methods)
                //{
                //    Console.WriteLine(method.GetParameters());
                //}
                
                Console.WriteLine(type.Name);
            }

            Console.ReadLine();
        }

        private static void TestIoC(IContainer container)
        {
            container.Register< IExternal >(() => new External());
            container.Register<IService, Service>();

            var service = container.Resolve<IService>();

            service.Do();
        }
    }
}
