using System;

namespace Contracts.Events
{
    public interface ICreatedOrderEvent
    {
        Guid Id { get; set; }
        Guid CustomerId { get; set; }

        int ItemsCount { get; set; }

        decimal Total { get; set; }
        decimal Vat { get; set; }

        int Status { get; set; }
    }
}