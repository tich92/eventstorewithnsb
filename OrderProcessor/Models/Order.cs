using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderProcessor.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public int ItemsCount { get; set; }
        public decimal Total { get; set; }
        public decimal Vat { get; set; }
        public OrderStatus Status { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public void CalculateOrder()
        {
            if (OrderItems == null)
                return;

            Total = OrderItems.Sum(o => o.Price * o.Quantity);
            ItemsCount = OrderItems.Sum(o => o.Quantity);

            Vat = Total * 0.2m;
        }

        public void Place()
        {
            Status = OrderStatus.Place;
        }
    }

    public enum OrderStatus
    {
        New = 1,
        Place = 2,
        Cancel = 3
    }
}