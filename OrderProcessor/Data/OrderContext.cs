using System.Data.Entity;
using OrderProcessor.Migrations;
using OrderProcessor.Models;

namespace OrderProcessor.Data
{
    public class OrderContext : DbContext
    {
        public OrderContext()
        {
            
        }
        
        public OrderContext(string configName) : base(configName)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<OrderContext, Configuration>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<OrderContext>(null);
            base.OnModelCreating(modelBuilder);
        }
        
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
    }
}