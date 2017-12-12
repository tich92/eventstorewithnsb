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
                cfg.CreateMap<ICreatedOrderEvent, CreateOrderCommand>();
                cfg.CreateMap<ICreatedOrderItemEvent, AddOrderItemCommand>();
                cfg.CreateMap<IPlacedOrderEvent, PlaceOrderCommand>();
            });

            Mapper = config.CreateMapper();
        }
    }
}