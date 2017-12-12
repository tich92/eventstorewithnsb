using System;

namespace Contracts.Commands
{
    public class CreateOrderCommand
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

        public int ItemsCount { get; set; }

        public decimal Total { get; set; }
        public decimal Vat { get; set; }

        public int Status { get; set; }
    }
}
