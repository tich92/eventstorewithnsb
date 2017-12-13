using System;

namespace Contracts.Commands
{
    public class CheckOutOrderCommand
    {
        public Guid OrderId { get; set; }
    }
}