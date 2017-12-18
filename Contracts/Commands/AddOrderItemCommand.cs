using System;

namespace Contracts.Commands
{
    public class AddOrderItemCommand : BaseCommand
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}