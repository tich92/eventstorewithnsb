using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace EventStoreContext.Tests
{
    [TestClass]
    public class ProjectionContextTests
    {
       

        [TestMethod]
        public async Task CreateProjectionTest()
        {
            var projectionManager = new ProjectionProvider();

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

            await projectionManager.CreateContinuousAsync(name, projectionQuery);

            await projectionManager.EnableAsync(name);

            var query = projectionManager.GetQueryAsync(name);

            Assert.IsNotNull(query);
            Assert.AreEqual(query, projectionQuery);
        }
        
        //[TestMethod]
        //public async Task UpdateProjectionTest()
        //{
        //    var projectionManager = new ProjectionContext();

        //    var name = "Order c0c8b62c-607e-4298-824c-1a73f7361f75";

        //    //var newQuery = @"fromStream('Order c0c8b62c-607e-4298-824c-1a73f7361f75')
        //    //                .when({
        //    //                    $init:function(){
        //    //                        return {
        //    //                            events: []
        //    //                        };
        //    //                    },
        //    //                    $any: function(state, event){
                                    
        //    //                        var metadata = JSON.parse(event.metadataRaw);
        //    //                        var timeStamp = metadata.TimeStamp;
                                    
        //    //                        if(timeStamp && Date.parse(timeStamp) >= Date.parse('2017-12-15 18:20:36')){
        //    //                            var data = {
        //    //                                eventType : event.eventType,
        //    //                                eventData : event.body,
        //    //                                timeStamp : timeStamp
        //    //                            };
            
        //    //                            state.events.push(data);
        //    //                        }
        //    //                    }
        //    //                });";

        //    //var currentProjection = await projectionManager.GetQueryAsync(name);

        //    //if (newQuery != currentProjection)
        //    //{
        //    //    await projectionManager.UpdateQueryAsync(name, newQuery, credentials);
        //    //}
        //}

        [TestMethod]
        public async Task GetByQueryTest()
        {
            var projectionName = "Order c0c8b62c-607e-4298-824c-1a73f7361f75";

            var projectionManager = new ProjectionProvider();

            var state = await projectionManager.GetStateAsync(projectionName);

            Assert.IsNotNull(state);
        }

        [TestMethod]
        public async Task GetAllStreamsProjectionTest()
        {
            var projectionContext = new ProjectionProvider();

            var data = await projectionContext.GetListOfOrderStreamsAsync();

            Assert.IsNotNull(data);
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