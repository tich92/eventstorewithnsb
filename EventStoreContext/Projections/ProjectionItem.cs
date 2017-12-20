namespace EventStoreContext.Projections
{
    public class ProjectionItem
    {
        public string Name { get; set; }
        public string Query { get; set; }

        public ProjectionItem(string name, string query)
        {
            Name = name;
            Query = query;
        }
    }
}