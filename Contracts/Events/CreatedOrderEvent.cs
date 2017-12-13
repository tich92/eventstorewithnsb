using System;

namespace Contracts.Events
{
    public class CreatedOrderEvent : ClientEvent
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public int ItemsCount { get; set; }
        public decimal Total { get; set; }
        public decimal Vat { get; set; }
        public int Status { get; set; }
    }
}