using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NServiceBus.Testing;
using OrderProcessor.Data;
using EventStoreContext;

namespace OrderProcessor.Tests
{
    [TestClass]
    public class OrderHandlerTests
    {
        private readonly OrderHandler orderHandler;

        private readonly EventContext eventContext;

        private readonly MappingTestConfig mappingTestConfig;

        const string StreamName = "Order c0c8b62c-607e-4298-824c-1a73f7361f75";

        public OrderHandlerTests()
        {
            eventContext = new EventContext();

            var mockOrderDbSet = new MockedDbContext<OrderContext>();

            mockOrderDbSet.MockTables();

            var mappingConfig = new MappingConfig();

            orderHandler = new OrderHandler(mappingConfig.Mapper, mockOrderDbSet.Object);
            mappingTestConfig = new MappingTestConfig();
        }

        private async Task<IEnumerable<EventModel>> GetEvents()
        {
            return await eventContext.ReadStreamEventsForwardAsync(StreamName);
        }

        [TestMethod]
        public async Task GettingEventsFromStreamTest()
        {
            var events = await GetEvents();

            foreach (var @event in events)
            {
                Assert.IsNotNull(@event);
            }
        }

        [TestMethod]
        public async Task HandleMessagesFromStoreAsync()
        {
            var events = await GetEvents();

            var handler = Test.Handler(orderHandler);
            
            foreach (var @event in events)
            {
                try
                {
                    var data = mappingTestConfig.Mapper.Map(@event, @event.GetType(), @event.Data.GetType());

                    handler.OnMessage(data);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw e;
                }
            }
        }
    }
}