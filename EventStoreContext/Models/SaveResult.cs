namespace EventStoreContext.Models
{
    public class SaveResult
    {
        public long NextExpectedVersion { get; set; }
        public long LogPosition { get; set; }

        public SaveResult(long nextExpectedVersion, long logPosition)
        {
            LogPosition = logPosition;
            NextExpectedVersion = nextExpectedVersion;
        }
    }
}