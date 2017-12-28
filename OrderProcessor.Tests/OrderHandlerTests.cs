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
using NBench;
using OrderProcessor.Handlers;

namespace OrderProcessor.Tests
{
    [TestClass]
    public class OrderHandlerTests : PerformanceTestStuite<OrderHandlerTests>
    {
        private readonly OrderHandler orderHandler;

        private readonly EventProvider eventContext;

        private readonly MappingTestConfig mappingTestConfig;

        private readonly OrderContext orderContext;

        private const string StreamName = "Order c0c8b62c-607e-4298-824c-1a73f7361f75";

        private readonly ProjectionProvider projectionContext;

        public OrderHandlerTests()
        {
            eventContext = new EventProvider();

            var mockOrderContext = new MockedDbContext<OrderContext>();

            projectionContext = new ProjectionProvider();

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
        [PerfBenchmark(RunMode = RunMode.Iterations, TestMode = TestMode.Measurement)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
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
        [PerfBenchmark(RunMode = RunMode.Iterations, TestMode = TestMode.Measurement)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        public async Task PerformAllEventsTest()
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            var streams = await projectionContext.GetListOfOrderStreamsAsync();

            Assert.IsNotNull(streams);
            Assert.IsTrue(streams.Items.Any());

            var eventProcessor =
                new ExecuteEventProcessor(orderContext, eventContext, mappingTestConfig.Mapper)
                {
                    MessageHandlerContext = new TestableMessageHandlerContext()
                };

            stopwatch.Stop();

            var readEventsTime = stopwatch.ElapsedMilliseconds;

            Debug.WriteLine($"Read {streams.Count} events at time {readEventsTime} ms");

            stopwatch.Start();

            foreach (var streamName in streams.Items)
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
            Debug.WriteLine(stopwatch.ElapsedMilliseconds);

            var time = (stopwatch.ElapsedMilliseconds / 1000) / 60;

            Debug.WriteLine(time);
        }

        [TestMethod]
        [PerfBenchmark(RunMode = RunMode.Iterations, TestMode = TestMode.Measurement)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        public async Task PerformAllEventsWithExpressionTest()
        {
            Stopwatch stopwatch = new Stopwatch();

            var list = await projectionContext.GetListOfOrderStreamsAsync();

            var eventProcessor =
                new ExecuteEventProcessor(orderContext, eventContext, mappingTestConfig.Mapper)
                {
                    MessageHandlerContext = new TestableMessageHandlerContext()
                };

            stopwatch.Start();

            foreach (var streamName in list.Items)
            {
                await eventProcessor.PerformEventsByStreamWithExpression(streamName, orderHandler);
            }

            stopwatch.Stop();
            Debug.WriteLine(stopwatch.ElapsedMilliseconds);
            
            var time = (stopwatch.ElapsedMilliseconds / 1000) / 60;

            Debug.WriteLine(time);
        }
    }
}