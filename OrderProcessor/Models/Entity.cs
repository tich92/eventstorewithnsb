namespace OrderProcessor.Models
{
    public abstract class Entity
    {
        /// <summary>
        /// Version of event in EventStore
        /// </summary>
        public long NextExpectedVersion { get; set; }

        /// <summary>
        /// Log position in EventStore
        /// </summary>
        public long LogPosition { get; set; }

        /// <summary>
        /// Update data from EventStore
        /// </summary>
        /// <param name="nextExpectedVersion"></param>
        /// <param name="logPosition"></param>
        public void UpdateEventData(long nextExpectedVersion, long logPosition)
        {
            NextExpectedVersion = nextExpectedVersion;
            LogPosition = logPosition;
        }
    }
}