using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TypeScanner;


namespace EventStoreContext.Tests
{
    [TestClass]
    public class EventStoreContextTest
    {
        private readonly EventContext eventContext;
        private readonly ITypeScanner typeScanner;

        public EventStoreContextTest()
        {
            eventContext = new EventContext();
            typeScanner = new TypeScanner.TypeScanner();
        }

        private IEnumerable<Assembly> TestGetAssemblyWithTypes()
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();

            loadedAssemblies.AddRange(toLoad.Select(path => AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));

            return loadedAssemblies;
        }

        private Type GetDerrivedType(string typeName)
        {
            return TestGetAssemblyWithTypes().SelectMany(o => o.GetTypes()).FirstOrDefault(o => o.FullName == typeName);

            //return Assembly
            //    .GetExecutingAssembly()
            //    .GetReferencedAssemblies()
            //    .Select(Assembly.Load)
            //    .SelectMany(x => x.GetTypes()).FirstOrDefault(x => x.FullName == typeName);
        }

        [TestMethod]
        public void GetTypeFromReferencedAssemblyTest()
        {
            var fullName = "Contracts.Events.CreatedOrderItemEvent";
            
            var type = GetDerrivedType(fullName);
            
            Assert.IsNotNull(type);
        }

        [TestMethod]
        public async Task ReadEventsTest()
        {
            var streamName = "Order c0c8b62c-607e-4298-824c-1a73f7361f75";

            var events = await eventContext.ReadStreamEventsBackward(streamName);

            foreach (var @event in events)
            {
                var eventType = @event.EventType;

                var data = JsonConvert.DeserializeObject(@event.EventData, GetDerrivedType(eventType));

                Assert.IsNotNull(@event);
            }
        }
    }
}
