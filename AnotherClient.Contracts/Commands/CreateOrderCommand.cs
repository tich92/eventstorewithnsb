using System;

namespace AnotherClient.Contracts.Commands
{
    public class CreateOrderCommand
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string BillingAddress { get; set; }

    }
}
