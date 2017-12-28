using System;
using System.Text;
using EventStore.ClientAPI;
using EventStoreContext.Models;
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
            var eventName = value.GetType().FullName;
            
            var metadata = new EventMetaData
            {
                TimeStamp = DateTime.UtcNow
            };

            var metadataJson = JsonConvert.SerializeObject(metadata);

            return new EventData(Guid.NewGuid(), eventName, true, data, Encoding.UTF8.GetBytes(metadataJson));
        }
    }
}
