using System;
using System.Collections.Generic;

namespace OrderProcessor.Models
{
    public class Customer : Entity
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }

        public string Phone { get; set; }
        public string CreatedDate { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}