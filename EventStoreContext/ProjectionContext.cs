using System;
using System.Net;
using System.Threading.Tasks;
using EventStore.ClientAPI.Common.Log;
using EventStore.ClientAPI.Embedded;
using EventStore.ClientAPI.Projections;
using EventStore.Common.Options;
using EventStore.Core;
using EventStoreContext.Helpers;
using EventStoreContext.Models;
using Newtonsoft.Json;

namespace EventStoreContext
{
    public class ProjectionContext
    {
        private readonly IPEndPoint tcp = new IPEndPoint(IPAddress.Loopback, 1113);
        private readonly IPEndPoint http = new IPEndPoint(IPAddress.Loopback, 2113);

        private readonly ProjectionsManager projectionManager;

        private ClusterVNode InitNode()
        {
            return EmbeddedVNodeBuilder.AsSingleNode()
                .RunInMemory()
                .WithExternalTcpOn(tcp)
                .WithExternalHttpOn(http)
                .RunProjections(ProjectionType.All)
                .StartStandardProjections()
                .Build();
        }

        public ProjectionContext()
        {
            var node = InitNode();
            node.StartAndWaitUntilReady().GetAwaiter().GetResult();

            projectionManager = new ProjectionsManager(new ConsoleLogger(), http, new TimeSpan(1, 0, 0));
        }

        public async Task<StreamList> GetResultOfStreamListAsync()
        {
            var result = await projectionManager.GetStateAsync("ListOrStreams", CredentialsHelper.Default);

            return JsonConvert.DeserializeObject<StreamList>(result);
        }


        public async Task CreateContinuousAsync(string name, string projectionQuery)
        {
            await projectionManager.CreateContinuousAsync(name, projectionQuery, CredentialsHelper.Default);
        }

        public async Task EnableAsync(string name)
        {
            await projectionManager.EnableAsync(name, CredentialsHelper.Default);
        }

        public async Task DisableAsync(string name)
        {
            await projectionManager.DisableAsync(name, CredentialsHelper.Default);
        }

        public async Task ResetAsync(string name)
        {
            await EnableAsync(name);
            await DisableAsync(name);
        }

        public async Task<string> GetStateAsync(string namne)
        {
            return await projectionManager.GetStateAsync(namne, CredentialsHelper.Default);
        }

        public async Task<string> GetQueryAsync(string name)
        {
            return await projectionManager.GetQueryAsync(name, CredentialsHelper.Default);
        }
    }
}