using System;

namespace Contracts.Commands
{
    public class CheckOutOrderCommand : BaseCommand
    {
        public Guid OrderId { get; set; }
    }
}