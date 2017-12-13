using EventStore.ClientAPI.SystemData;

namespace EventStoreContext.Helpers
{
    internal static class CredentialsHelper
    {
        internal static UserCredentials Default { get; } = new UserCredentials("admin", "changeit");
    }
}
