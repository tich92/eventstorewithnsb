using System;
using System.Threading;
using System.Threading.Tasks;
using Contracts.Commands;
using NServiceBus;

namespace Client
{
    public class OrderProducer
    {
        private readonly IMessageSession messageSession;
        private readonly OrderPersistence orderPersistence;

        public OrderProducer(IMessageSession messageSession, 
            OrderPersistence orderPersistence)
        {
            this.messageSession = messageSession;
            this.orderPersistence = orderPersistence;
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

            var customer = new CreateCustomerCommand()
            {
                Id = order.CustomerId,
                Email = $"{order.CustomerId}@test.com",
                CreatedDate = DateTime.Now,
                FullName = $"{order.CustomerId}",
                Phone = "+380631472589"
            };

            await messageSession.Send(customer);

            Thread.Sleep(2000);

            await messageSession.Send(order);

            orderPersistence.Add(order.Id);
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

            await messageSession.Send(command);
        }

        public async Task PlaceOrder(Guid orderId)
        {
            var command = new PlaceOrderCommand(orderId);

            await messageSession.Send(command);
        }

        public async Task CancelOrder(Guid orderId)
        {
            await messageSession.Send(new CancelOrderCommand(orderId));
        }

        //39a1928d-51e7-4191-9860-7f31215d644c
        public async Task CreateCustomer(Guid customerId)
        {
            var command = new CreateCustomerCommand
            {
                Id = customerId,
                Email = $"{customerId}@test.com",
                CreatedDate = DateTime.Now,
                FullName = $"{customerId}",
                Phone = "+380931234567"
            };

            await messageSession.Send(command);
        }

        public async Task GenerateTestEvents()
        {
            int iterations = 2000;

            Random rndItems = new Random();
            
            for (int i = 0; i < iterations; i++)
            {
                var orderId = Guid.NewGuid();

                var customerId = Guid.NewGuid();

                var customerCommand = new CreateCustomerCommand
                {
                    Id = customerId,
                    FullName = customerId.ToString(),
                    CreatedDate = DateTime.Now,
                    Email = $"{customerId}@test.com",
                    Phone = "123564"
                };

                await messageSession.Send(customerCommand);

                Console.WriteLine($"Sent customer with id {customerId}");

                Thread.Sleep(2000);

                var order = new CreateOrderCommand
                {
                    Id = orderId,
                    CustomerId = customerId,
                    ItemsCount = 0,
                    Status = 1,
                    Total = 0,
                    Vat = 0
                };

                await messageSession.Send(order);

                Console.WriteLine($"Sent order with id {orderId}");
                
                int countItems = rndItems.Next(6, 12);

                for (int j = 0; j < countItems; j++)
                {
                    var quantity = rndItems.Next(1, 12);

                    var orderItem = new AddOrderItemCommand
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderId,
                        Quantity = quantity,
                        Price = quantity * 10
                    };

                    await messageSession.Send(orderItem);

                    Console.WriteLine($"Sent order item with id {orderItem}");
                }

                if (i % 2 == 0)
                {
                    var place = new PlaceOrderCommand(orderId);

                    await messageSession.Send(place);

                    Console.WriteLine($"Sent place order with id {orderId}");
                }
                else
                {
                    var cancel = new CancelOrderCommand(orderId);
                    await messageSession.Send(cancel);

                    Console.WriteLine($"Sent cancel order with id {orderId}");

                }
            }
        }
    }
}