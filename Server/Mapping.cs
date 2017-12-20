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
                cfg.CreateMap<CreatedOrderEvent, CreateOrderCommand>()
                    .ForMember(dest => dest.NextExpectedVersion, opt => opt.MapFrom(src => src.NextExpectedVersion));

                cfg.CreateMap<CreatedOrderItemEvent, AddOrderItemCommand>()
                    .ForMember(dest => dest.NextExpectedVersion, opt => opt.MapFrom(src => src.NextExpectedVersion));

                cfg.CreateMap<PlacedOrderEvent, PlaceOrderCommand>()
                    .ForMember(dest => dest.NextExpectedVersion, opt => opt.MapFrom(src => src.NextExpectedVersion));

                cfg.CreateMap<CancelOrderEvent, CancelOrderCommand>()
                    .ForMember(dest => dest.NextExpectedVersion, opt => opt.MapFrom(src => src.NextExpectedVersion));

                cfg.CreateMap<CreateCustomerEvent, CreateCustomerCommand>()
                    .ForMember(dest => dest.NextExpectedVersion, opt => opt.MapFrom(src => src.NextExpectedVersion));
            });

            Mapper = config.CreateMapper();
        }
    }
}