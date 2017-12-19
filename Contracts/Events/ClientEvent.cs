namespace Contracts.Events
{
    public class ClientEvent
    {
        public long NextExpectedVersion { get; set; }
        public long LogPosition { get; set; }
    }
}
