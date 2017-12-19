namespace Contracts
{
    /// <summary>
    /// Base command class
    /// </summary>
    public abstract class BaseCommand
    {
        // Version of event from EventStore
        public long NextExpectedVersion { get; set; }

        // Position in EventStore
        public long LogPosition { get; set; }
    }
}