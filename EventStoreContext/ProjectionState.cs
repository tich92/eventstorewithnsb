using System.Collections.Generic;
using Newtonsoft.Json;

namespace EventStoreContext
{
    public class ProjectionState
    {
        [JsonProperty("events")]
        //public EventModel[] Events { get; set; }
        public object[] Events { get; set; }
    }
}
