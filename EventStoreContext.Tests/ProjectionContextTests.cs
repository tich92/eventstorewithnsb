using System;
using System.Linq;
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
using Newtonsoft.Json;

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

            var name = "Order c0c8b62c-607e-4298-824c-1a73f7361f75";
            
            var projectionQuery = @"fromStream('Order c0c8b62c-607e-4298-824c-1a73f7361f75')
                            .when({
                                $init:function(){
                                    return {
                                        events: []
                                    };
                                },
                                $any: function(state, event){
                                    
                                    var metadata = JSON.parse(event.metadataRaw);
                                    var timeStamp = metadata.TimeStamp;
                                    
                                    if(Date.parse(timeStamp) >= Date.parse('2017-12-15 18:20:36')){
                                        var data = {
                                            eventType : event.eventType,
                                            eventData : event,
                                            timeStamp : timeStamp
                                        };
            
                                        state.events.push(data);
                                    }
                                }
                            });";

            await projectionManager.CreateContinuousAsync(name, projectionQuery, credentials);

            await projectionManager.EnableAsync(name, credentials);

            var query = projectionManager.GetQueryAsync(name, credentials);

            Assert.IsNotNull(query);
            Assert.AreEqual(query, projectionQuery);
        }

        [TestMethod]
        public async Task GetProjectionStateTest()
        {
            var projectionManager = await InitProjectionManager();

            var all = await projectionManager.ListAllAsync(credentials);

            Assert.IsNotNull(all);
            Assert.IsTrue(all.Any());

            foreach (var projection in all)
            {
                var state = await projectionManager.GetStateAsync(projection.Name, credentials);

                Assert.IsNotNull(state);
            }
        }

        [TestMethod]
        public async Task UpdateProjectionTest()
        {
            var projectionManager = await InitProjectionManager();
            
            var name = "Order c0c8b62c-607e-4298-824c-1a73f7361f75";

            var newQuery = @"fromStream('Order c0c8b62c-607e-4298-824c-1a73f7361f75')
                            .when({
                                $init:function(){
                                    return {
                                        events: []
                                    };
                                },
                                $any: function(state, event){
                                    
                                    var metadata = JSON.parse(event.metadataRaw);
                                    var timeStamp = metadata.TimeStamp;
                                    
                                    if(timeStamp && Date.parse(timeStamp) >= Date.parse('2017-12-15 18:20:36')){
                                        var data = {
                                            eventType : event.eventType,
                                            eventData : event.body,
                                            timeStamp : timeStamp
                                        };
            
                                        state.events.push(data);
                                    }
                                }
                            });";

            var currentProjection = await projectionManager.GetQueryAsync(name);

            if (newQuery != currentProjection)
            {
                await projectionManager.UpdateQueryAsync(name, newQuery, credentials);
            }
        }

        [TestMethod]
        public async Task GetByQueryTest()
        {
            var projectionName = "Order c0c8b62c-607e-4298-824c-1a73f7361f75";

            var projectionManager = await InitProjectionManager();

            var state = await projectionManager.GetStateAsync(projectionName, credentials);

            Assert.IsNotNull(state);
        }

        [TestMethod]
        public async Task GetResultTest()
        {
            var projectionManager = await InitProjectionManager();
            var projectionName = "Order c0c8b62c-607e-4298-824c-1a73f7361f75";

            var result = await projectionManager.GetResultAsync(projectionName, credentials);
            
            Assert.IsNotNull(result);
        }
    }

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
            DateParseHandling = DateParseHandling.None,
        };
    }
}