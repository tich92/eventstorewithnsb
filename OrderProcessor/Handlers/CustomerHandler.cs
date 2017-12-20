using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Events;
using NServiceBus;
using OrderProcessor.Data;
using OrderProcessor.Models;

namespace OrderProcessor.Handlers
{
    public class CustomerHandler : IHandleMessages<CreateCustomerEvent>
    {
        private readonly OrderContext orderContext;
        private readonly IMapper mapper;

        public CustomerHandler(OrderContext orderContext, IMapper mapper)
        {
            this.orderContext = orderContext;
            this.mapper = mapper;
        }

        public async Task Handle(CreateCustomerEvent message, IMessageHandlerContext context)
        {
            var customer = mapper.Map<Customer>(message);

            if(IsExist(message.Id))
                return;

            orderContext.Customers.Add(customer);

            await orderContext.SaveChangesAsync();
        }

        private bool IsExist(Guid id)
        {
            var customer = orderContext.Customers.FirstOrDefault(o => o.Id == id);

            return customer != null;
        }
    }
}