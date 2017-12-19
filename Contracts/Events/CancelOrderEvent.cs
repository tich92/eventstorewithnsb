using System;

namespace Contracts.Events
{
    public class CancelOrderEvent : ClientEvent
    {
        public Guid OrderId { get; set; }
    }
}