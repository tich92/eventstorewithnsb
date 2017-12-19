using AutoMapper;
using Contracts.Commands;
using Contracts.Events;

namespace Server
{
    public class Mapping
    {
        public IMapper Mapper { get; set; }

        public Mapping()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreatedOrderEvent, CreateOrderCommand>();
                cfg.CreateMap<CreatedOrderItemEvent, AddOrderItemCommand>();
                cfg.CreateMap<PlacedOrderEvent, PlaceOrderCommand>();
                cfg.CreateMap<CancelOrderEvent, CancelOrderCommand>();
            });

            Mapper = config.CreateMapper();
        }
    }
}