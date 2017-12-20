using System.Collections.Generic;

namespace EventStoreContext.Projections
{
    public class ProjectionList
    {
        public List<ProjectionItem> Items { get; set; }

        public ProjectionList()
        {
            Items = new List<ProjectionItem>();
        }

        public void Add(ProjectionItem item)
        {
            Items.Add(item);
        }

        public void Add(string name, string query)
        {
            Items.Add(new ProjectionItem(name, query));
        }
    }
}