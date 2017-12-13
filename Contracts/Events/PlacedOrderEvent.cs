using System;

namespace Contracts.Events
{
    public class PlacedOrderEvent : ClientEvent
    {
        public Guid OrderId { get; set; }
    }
}