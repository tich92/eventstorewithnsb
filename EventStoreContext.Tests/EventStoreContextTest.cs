using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EventStoreContext.Tests
{
    [TestClass]
    public class EventStoreContextTest
    {
        private readonly EventContext eventContext;
        
        public EventStoreContextTest()
        {
            eventContext = new EventContext();   
        }

        private IEnumerable<Assembly> GetReferencedTypes()
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();

            foreach (var path in toLoad)
            {
                loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path)));
            }

            return loadedAssemblies;
        }

        private Type GetTypeByName(string name)
        {
            var references = GetReferencedTypes();

            return references.SelectMany(t => t.GetTypes()).FirstOrDefault(t => t.FullName == name);
        }

        [TestMethod]
        public void GetAssemblyTypeTest()
        {
            var fullName = "Contracts.Events.CreatedOrderItemEvent";

            var type = GetTypeByName(fullName);

            Assert.IsNotNull(type);
        }

        [TestMethod]
        public async Task ReadEventsFromStreamTestAsync()
        {
            var streamName = "Order c0c8b62c-607e-4298-824c-1a73f7361f75";

            var events = await eventContext.ReadStreamEventsBackwardAsync(streamName);

            foreach (var @event in events)
            {
                Assert.IsNotNull(@event);
            }
        }

        [TestMethod]
        public async Task ReadEventsForwardsFromStreamTest()
        {
            var streamName = "Order c0c8b62c-607e-4298-824c-1a73f7361f75";

            var events = await eventContext.ReadStreamEventsForwardAsync(streamName);

            foreach (var @event in events)
            {
                Assert.IsNotNull(@event);
            }
        }

        [TestMethod]
        public async Task ReadAllEventsForwardTest()
        {
            var events = await eventContext.ReadAllEventsForwardAsync();

            Assert.IsNotNull(events);

            foreach (var @event in events)
            {
                Assert.IsNotNull(@event);
            }
        }

        [TestMethod]
        public async Task ReadAllEventsBackwardTest()
        {
            var events = await eventContext.ReadAllEventsBackwardAsync();

            Assert.IsNotNull(events);

            foreach (var @event in events)
            {
                Assert.IsNotNull(@event);
            }
        }

        [TestMethod]
        public async Task ReadAllStreamsTest()
        {
            var streams = await eventContext.ReadAllStreamsAsync();

            Assert.IsNotNull(streams);
        }
    }
}