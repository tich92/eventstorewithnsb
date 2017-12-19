using System;

namespace OrderProcessor.Models
{
    public class OrderItem : Entity
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Order Order { get; set; }
    }
}