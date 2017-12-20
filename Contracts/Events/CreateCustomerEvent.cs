using System;

namespace Contracts.Events
{
    public class CreateCustomerEvent : ClientEvent
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }

        public string Phone { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}