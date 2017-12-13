using System;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EventStoreContext.Helpers
{
    internal static class EventHelpers
    {
        internal static EventData ToEvent<T>(this T value)
        {
            if(value == null)
                throw new ArgumentNullException(nameof(value));

            var json = JsonConvert.SerializeObject(value);
            var data = Encoding.UTF8.GetBytes(json);
            var eventName = value.GetType().Name;

            return new EventData(Guid.NewGuid(), eventName, true, data, null);
        }

        internal static EventModel ParseEvent(this RecordedEvent @event)
        {
            if(@event == null)
                throw new ArgumentNullException(nameof(@event));

            var value = Encoding.UTF8.GetString(@event.Data);
            
            return new EventModel()
            {
                EventData = value,
                EventType = @event.EventType
            };
        }
    }
}
