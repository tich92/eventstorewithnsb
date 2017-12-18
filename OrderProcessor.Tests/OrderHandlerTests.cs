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

            var mappingConfig = new MappingConfig();
            
            orderHandler = new OrderHandler(mappingConfig.Mapper, mockOrderDbSet.Object);
        }

        public static DbSet<T> MockDbSet<T>(List<T> table) where T : class
        {
            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(q => q.Provider).Returns(() => table.AsQueryable().Provider);
            dbSet.As<IQueryable<T>>().Setup(q => q.Expression).Returns(() => table.AsQueryable().Expression);
            dbSet.As<IQueryable<T>>().Setup(q => q.ElementType).Returns(() => table.AsQueryable().ElementType);
            dbSet.As<IQueryable<T>>().Setup(q => q.GetEnumerator()).Returns(() => table.AsQueryable().GetEnumerator());
            dbSet.Setup(set => set.Add(It.IsAny<T>())).Callback<T>(table.Add);
            dbSet.Setup(set => set.AddRange(It.IsAny<IEnumerable<T>>())).Callback<IEnumerable<T>>(table.AddRange);
            dbSet.Setup(set => set.Remove(It.IsAny<T>())).Callback<T>(t => table.Remove(t));
            dbSet.Setup(set => set.RemoveRange(It.IsAny<IEnumerable<T>>())).Callback<IEnumerable<T>>(ts =>
            {
                foreach (var t in ts)
                {
                    table.Remove(t);
                }
            });

            return dbSet.Object;
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
