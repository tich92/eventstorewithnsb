using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EventStore.ClientAPI;

using EventStoreContext.Helpers;

namespace EventStoreContext
{
    public class EventContext
    {
        private readonly IEventStoreConnection eventStoreConnection;

        private const int PageSize = 100;

        private static IPEndPoint DefaultTcp()
        {
            return CreateIpEndPoint(1113);
        }

        private static IPEndPoint CreateIpEndPoint(int port)
        {
            var address = IPAddress.Parse("127.0.0.1");
            return new IPEndPoint(address, port);
        }

        public EventContext()
        {
            eventStoreConnection = EventStoreConnection.Create(DefaultTcp());
            eventStoreConnection.ConnectAsync().GetAwaiter().GetResult();
        }

        public async Task Add<T>(string streamName, T value)
            where T : class
        {
            var @event = value.ToEvent();

            await eventStoreConnection.AppendToStreamAsync(streamName, ExpectedVersion.Any, @event);
        }

        public async Task<long?> GetLastEventNumber(string streamName)
        {
            var lastEvent = await eventStoreConnection.ReadEventAsync(streamName, -1, false, CredentialsHelper.Default);

            return lastEvent?.Event?.OriginalEventNumber;
        }

        public async Task<IEnumerable<object>> ReadStreamEventsBackward(string streamName)
        {
            var lastEventNumber = await GetLastEventNumber(streamName);

            return lastEventNumber == null ? new List<object>() : await ReadResult(streamName, lastEventNumber.Value);
        }

        public async Task<IEnumerable<object>> ReadStreamEventsForward(string streamName)
        {
            var records =
                await eventStoreConnection.ReadStreamEventsForwardAsync(streamName, 0, PageSize, false,
                    CredentialsHelper.Default);

            return records.Events.Select(@event => @event.Event.ParseEvent()).ToList();
        }

        private async Task<IEnumerable<object>> ReadResult(string streamName, long lastEventNumber)
        {
            var eventList = new List<object>();

            do
            {
                var result = await eventStoreConnection.ReadStreamEventsBackwardAsync(streamName, lastEventNumber,
                    PageSize,
                    false, CredentialsHelper.Default);

                eventList.AddRange(result.Events.Select(o => o.Event.ParseEvent()));
                lastEventNumber = result.NextEventNumber;
            } while (lastEventNumber != -1);

            return eventList;
        }
    }
}