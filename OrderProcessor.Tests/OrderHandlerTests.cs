using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NServiceBus.Testing;
using OrderProcessor.Data;
using EventStoreContext;
using OrderProcessor.Models;
using System.Linq.Expressions;
using System;

namespace OrderProcessor.Tests
{
    [TestClass]
    public class OrderHandlerTests
    {
        private readonly OrderHandler orderHandler;

        private readonly EventContext eventContext;

        const string StreamName = "Order c0c8b62c-607e-4298-824c-1a73f7361f75";

        public OrderHandlerTests()
        {
            eventContext = new EventContext();

            var mockOrderDbSet = new MockedDbContext<OrderContext>();

            mockOrderDbSet.MockTables();

            var mappingConfig = new MappingConfig();
            
            orderHandler = new OrderHandler(mappingConfig.Mapper, mockOrderDbSet.Object);
        }
        
        private async Task<IEnumerable<object>> GetEvents()
        {
            return await eventContext.ReadStreamEventsForward(StreamName);
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
            
            foreach (var @event in events)
            {

                Test.Handler(orderHandler).OnMessage(@event);
            }
        }
    }
}
