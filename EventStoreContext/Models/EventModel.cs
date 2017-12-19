namespace EventStoreContext.Models
{
    public class EventModel
    {
        public object Data { get; set; }
        public long EventNumber { get; set; }

        public EventModel(object data, long eventNumber)
        {
            Data = data;
            EventNumber = eventNumber;
        }
    }
}