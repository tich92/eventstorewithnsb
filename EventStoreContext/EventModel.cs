using Newtonsoft.Json;

namespace EventStoreContext
{
    public class EventModel
    {
        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("eventData")]
        public string EventData { get; set; }
    }
}