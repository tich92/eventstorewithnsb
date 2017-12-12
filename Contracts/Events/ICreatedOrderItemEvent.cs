using System;

namespace Contracts.Events
{
    public interface ICreatedOrderItemEvent
    {
        Guid Id { get; set; }
        Guid OrderId { get; set; }

        int Quantity { get; set; }
        decimal Price { get; set; }
    }
}