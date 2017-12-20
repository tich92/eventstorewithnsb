using System;

namespace Contracts.Commands
{
    public class CreateCustomerCommand : BaseCommand
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }

        public string Phone { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}