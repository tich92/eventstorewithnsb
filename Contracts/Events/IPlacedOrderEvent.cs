using System;

namespace Contracts.Events
{
    public interface IPlacedOrderEvent
    {
        Guid OrderId { get; set; }
    }
}