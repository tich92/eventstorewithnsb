using System;

namespace Contracts.Events
{
    public class CreatedOrderItemEvent : ClientEvent
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}