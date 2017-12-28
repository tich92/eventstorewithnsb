using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Events;
using NServiceBus;
using NServiceBus.Logging;
using OrderProcessor.Data;
using OrderProcessor.Models;

namespace OrderProcessor.Handlers
{
    public class CustomerHandler : IHandleMessages<CreateCustomerEvent>
    {
        private ILog log = LogManager.GetLogger<CustomerHandler>();

        private readonly OrderContext orderContext;
        private readonly IMapper mapper;

        public CustomerHandler(OrderContext orderContext, IMapper mapper)
        {
            this.orderContext = orderContext;
            this.mapper = mapper;
        }

        public async Task Handle(CreateCustomerEvent message, IMessageHandlerContext context)
        {
            log.Debug($"Handled new customer with Id {message.Id}");

            var customer = mapper.Map<Customer>(message);

            if(IsExist(message.Id))
                return;

            orderContext.Customers.Add(customer);

            await orderContext.SaveChangesAsync();

            log.Debug($"Handling new customer with Id {message.Id} successful performed");
        }

        private bool IsExist(Guid id)
        {
            var customer = orderContext.Customers.FirstOrDefault(o => o.Id == id);

            return customer != null;
        }
    }
}