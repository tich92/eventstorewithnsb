using System.Threading.Tasks;

namespace EventStoreContext.Projections
{
    public class CustomProjectionProvider
    {
        private readonly ProjectionProvider projectionContext;

        public ProjectionList ProjectionList { get; set; }

        public CustomProjectionProvider(ProjectionProvider projectionContext)
        {
            this.projectionContext = projectionContext;

            ProjectionList = new ProjectionList();

            AddCustomProjections();
        }

        private void AddCustomProjections()
        {
            ProjectionList.Add("ListOfCustomerId",
                @"fromAll().
                   when({
                       $init: function(){
                             return{
                                count: 0,
                                items : []
                             };
                       },
                        $any : function(state,event) {
                            if((event.eventType == 'Contracts.Events.CreatedOrderEvent'
                            || event.eventType == 'CreatedOrderEvent')){
                                state.count++;
                            state.items.push(event.body.CustomerId);  
                            }
                   }});");

            ProjectionList.Add("ListOfCustomerStreams",
                @"fromAll().
                   when({
                       $init: function(){
                             return{
                                count: 0,
                                items : []
                             };
                       },
                        $any : function(state,event) {
                            if (event.eventType && !event.eventType.startsWith('$') 
                                    && event.streamId.startsWith('Customer')){
                                if(!state.items.includes(event.streamId)){
                                    state.count++;
                                    state.items.push(event.streamId);    
                                }
                            }
                   }});");

            ProjectionList.Add("ListOfOrderStreams",
                @"fromAll().
                   when({
                       $init: function(){
                             return{
                                count: 0,
                                items : []
                             };
                       },
                        $any : function(state,event) {
                            if (event.eventType && !event.eventType.startsWith('$') 
                                    && event.streamId.startsWith('Order')){
                                if(!state.items.includes(event.streamId)){
                                    state.count++;
                                    state.items.push(event.streamId);    
                                }
                            }
                   }});");
        }

        public async Task RunProjections()
        {
            foreach (var item in ProjectionList.Items)
            {
                var query = await projectionContext.GetQueryAsync(item.Name);

                if (query == null)
                    await projectionContext.CreateContinuousAsync(item.Name, item.Query);
                else if (query != item.Query)
                    await projectionContext.UpdateProjectionAsync(item.Name, item.Query);
            }
        }
    }
}