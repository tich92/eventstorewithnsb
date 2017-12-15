using System;
using System.Net;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Embedded;
using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.SystemData;
using EventStore.Common.Options;
using EventStore.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EventStore.ClientAPI.Common.Log;
using EventStore.Projections.Core.Services.Management;

namespace EventStoreContext.Tests
{
    [TestClass]
    public class ProjectionContextTests
    {
        private ClusterVNode node;
        private readonly IPEndPoint tcp = new IPEndPoint(IPAddress.Loopback, 1113);
        private readonly IPEndPoint http = new IPEndPoint(IPAddress.Loopback, 2113);
        private readonly UserCredentials credentials = new UserCredentials("admin", "changeit");

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

        private async Task<ProjectionsManager> InitProjectionManager()
        {
            node = InitNode();
            node = await node.StartAndWaitUntilReady();

            return new ProjectionsManager(new ConsoleLogger(), http, new TimeSpan(1, 0, 0));
        }

        [TestMethod]
        public async Task TestSetupNode()
        {
            try
            {
                node = InitNode();
                node = await node.StartAndWaitUntilReady();

                Assert.IsNotNull(node);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [TestMethod]
        public async Task ConnectionTest()
        {
            node = InitNode();
            node = await node.StartAndWaitUntilReady();

            var connection = EmbeddedEventStoreConnection.Create(node,
                ConnectionSettings.Create()
                    .EnableVerboseLogging()
                    .UseConsoleLogger()
                    .SetDefaultUserCredentials(credentials)
                    .Build());

            await connection.ConnectAsync();
            
            connection.Connected += (sender, args) =>
            {
                connection = args.Connection;
            };

            Assert.IsNotNull(connection);
        }

        

        [TestMethod]
        public async Task GetAllProjectionsTest()
        {
            var projectionManager = await InitProjectionManager();

            var list = await projectionManager.ListAllAsync(credentials);

            Assert.IsNotNull(list);
        }

        [TestMethod]
        public async Task CreateProjectionTest()
        {
            var projectionManager = await InitProjectionManager();

            //string name = ""

            //projectionManager.CreateContinuousAsync()
        }
    }
}