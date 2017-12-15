using System;

namespace AnotherClient.Models
{
    public class Order
    {
        public int Id { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public int ItemsCount { get; set; }
    }
}