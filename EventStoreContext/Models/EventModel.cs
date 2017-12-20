namespace EventStoreContext.Models
{
    public class EventModel
    {
        public object Data { get; set; }
        public long NextExpectedVersion { get; set; }
        public long LogPosition { get; set; }

        public EventModel(object data, long eventNumber, long logPosition)
        {
            Data = data;
            NextExpectedVersion = eventNumber;
            LogPosition = logPosition;
        }
    }
}