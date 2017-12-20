using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NServiceBus.Testing;
using OrderProcessor.Data;
using EventStoreContext;
using EventStoreContext.Models;
using OrderProcessor.Handlers;

namespace OrderProcessor.Tests
{
    [TestClass]
    public class OrderHandlerTests
    {
        private readonly OrderHandler orderHandler;

        private readonly EventContext eventContext;

        private readonly MappingTestConfig mappingTestConfig;

        private readonly OrderContext orderContext;

        const string StreamName = "Order c0c8b62c-607e-4298-824c-1a73f7361f75";

        public OrderHandlerTests()
        {
            eventContext = new EventContext();

            var mockOrderContext = new MockedDbContext<OrderContext>();

            var projectionContext = new ProjectionContext();

            mockOrderContext.MockTables();

            orderContext = mockOrderContext.Object;

            var mappingConfig = new MappingConfig();

            orderHandler = new OrderHandler(mappingConfig.Mapper, mockOrderContext.Object, eventContext, projectionContext);
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
                    throw;
                }
            }
        }

        [TestMethod]
        public async Task PerformAllEventsTest()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var streams = await eventContext.GetSrteamListAsync();

            Assert.IsNotNull(streams);
            Assert.IsTrue(streams.Any());

            var eventProcessor = new ExecuteEventProcessor(orderContext,eventContext, mappingTestConfig.Mapper);
            eventProcessor.MessageHandlerContext = new TestableMessageHandlerContext();

            foreach (var streamName in streams)
            {
                try
                {
                    await eventProcessor.PerformEventsByStreamAsync(streamName, orderHandler);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }

            stopwatch.Stop();
            Debug.WriteLine(stopwatch.Elapsed);

        }
    }
}