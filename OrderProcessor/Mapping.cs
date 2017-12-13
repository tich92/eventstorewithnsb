using AutoMapper;
using Contracts.Events;
using OrderProcessor.Models;

namespace OrderProcessor
{
    public class Mapping
    {
        public IMapper Mapper { get; set; }

        public Mapping()
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreatedOrderEvent, Order>()
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore());
                cfg.CreateMap<CreatedOrderItemEvent, OrderItem>()
                .ForMember(dest => dest.Order, opt => opt.Ignore());
            });

            Mapper = mappingConfig.CreateMapper();
        }
    }
}