using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStoreContext.Helpers;
using EventStoreContext.Models;

namespace EventStoreContext
{
    public class EventProvider
    {
        private readonly IEventStoreConnection eventStoreConnection;

        private const int PageSize = 4096;

        private static IPEndPoint DefaultTcp()
        {
            return CreateIpEndPoint(1113);
        }

        private static IPEndPoint CreateIpEndPoint(int port)
        {
            var address = IPAddress.Parse("127.0.0.1");
            return new IPEndPoint(address, port);
        }

        public EventProvider()
        {
            eventStoreConnection = EventStoreConnection.Create(DefaultTcp());
            eventStoreConnection.ConnectAsync().GetAwaiter().GetResult();
        }

        public async Task<SaveResult> AddAsync<T>(string streamName, T value)
            where T : class
        {
            var @event = value.ToEvent();

            var result = await eventStoreConnection.AppendToStreamAsync(streamName, ExpectedVersion.Any, @event);
            return new SaveResult(result.NextExpectedVersion, result.LogPosition.CommitPosition);
        }

        public async Task<long?> GetLastEventNumberAsync(string streamName)
        {
            var lastEvent = await eventStoreConnection.ReadEventAsync(streamName, -1, false, CredentialsHelper.Default);

            return lastEvent?.Event?.OriginalEventNumber;
        }

        public async Task<IEnumerable<EventModel>> ReadStreamEventsBackwardAsync(string streamName)
        {
            var lastEventNumber = await GetLastEventNumberAsync(streamName);

            return lastEventNumber == null ? new List<EventModel>() : await ReadResult(streamName, lastEventNumber.Value);
        }

        public async Task<IEnumerable<EventModel>> ReadStreamEventsForwardAsync(string streamName)
        {
            var records =
                await eventStoreConnection.ReadStreamEventsForwardAsync(streamName, 0, PageSize, false,
                    CredentialsHelper.Default);

            return records.Events.Select(@event => @event.Event.ParseEvent()).ToList();
        }

        public async Task<IEnumerable<ResolvedEvent>> ReadStreamAsync(string streamName)
        {
            var records =
                await eventStoreConnection.ReadStreamEventsForwardAsync(streamName, 0, PageSize, false,
                    CredentialsHelper.Default);
            return records.Events;
        }

        public async Task<IEnumerable<EventModel>> ReadAllEventsForwardAsync()
        {
            var records =
                await eventStoreConnection.ReadAllEventsForwardAsync(Position.Start, PageSize, false,
                    CredentialsHelper.Default);

            return records.Events.Select(@event => @event.Event.ParseEvent()).ToList();
        }

        public async Task<IEnumerable<EventModel>> ReadAllEventsBackwardAsync()
        {
            var records =
                await eventStoreConnection.ReadAllEventsBackwardAsync(Position.Start, PageSize, false,
                    CredentialsHelper.Default);

            return records.Events.Select(@event => @event.Event.ParseEvent()).ToList();
        }

        public async Task<List<string>> GetSrteamListAsync()
        {
            var streams = await eventStoreConnection.ReadAllEventsForwardAsync(Position.Start, PageSize, false,
                CredentialsHelper.Default);

            //TODO: FOR TESTING READ STREAMS!!!
            var streamList = streams.Events.Where(s => s.Event.EventStreamId.StartsWith("Order"))
                .Select(s => s.Event.EventStreamId).Distinct().ToList();

            return streamList;
        }

        private async Task<IEnumerable<EventModel>> ReadResult(string streamName, long lastEventNumber)
        {
            var eventList = new List<EventModel>();

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