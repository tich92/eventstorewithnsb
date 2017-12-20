using System.Collections.Generic;
using Newtonsoft.Json;

namespace EventStoreContext.Models
{
    public class ProjectionResult
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("items")]
        public IEnumerable<string> Items { get; set; }
    }
}
