using System;
using System.Threading.Tasks;
using Contracts.Commands;
using NServiceBus;

namespace Client
{
    public class OrderProducer
    {
        private readonly IMessageSession _messageSession;
        private readonly OrderPersistence _orderPersistence;

        public OrderProducer(IMessageSession messageSession, 
            OrderPersistence orderPersistence)
        {
            _messageSession = messageSession;
            _orderPersistence = orderPersistence;
        }

        public async Task CreateOrderAsync()
        {
            var order = new CreateOrderCommand
            {
                Id = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                ItemsCount = 0,
                Total = 0,
                Vat = 0,
                Status = 1
            };

            await _messageSession.Send(order);

            _orderPersistence.Add(order.Id);
        }

        public async Task CreateOrderItem(Guid orderId, int quantity, decimal price)
        {
            var command = new AddOrderItemCommand
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                Price = price,
                Quantity = quantity
            };

            await _messageSession.Send(command);
        }

        public async Task PlaceOrder(Guid orderId)
        {
            var command = new PlaceOrderCommand(orderId);

            await _messageSession.Send(command);
        }
    }
}