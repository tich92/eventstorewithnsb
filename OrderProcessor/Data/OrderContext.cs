using System;
using System.Data.Entity;
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
            
        }

        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
    }
}
