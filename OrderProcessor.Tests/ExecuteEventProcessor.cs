using System.Threading.Tasks;
using EventStoreContext;

namespace OrderProcessor.Tests
{
    using Data;

    public class ExecuteEventProcessor
    {
        private OrderContext orderContext;
        private EventContext eventContext;

        public ExecuteEventProcessor(OrderContext orderContext, EventContext eventContext)
        {
            this.orderContext = orderContext;
            this.eventContext = eventContext;
        }

        public async Task DropDataAsync()
        {
            await orderContext.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[Orders]");
            await orderContext.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[OrderItems]");
        }

        //public async Task ExecuteDataFromStore()
        //{
        //    var events
        //}
    }
}