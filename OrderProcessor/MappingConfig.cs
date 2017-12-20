using AutoMapper;
using Contracts.Events;
using EventStoreContext.Models;
using OrderProcessor.Models;

namespace OrderProcessor
{
    public class MappingConfig
    {
        public IMapper Mapper { get; set; }

        public MappingConfig()
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreatedOrderEvent, Order>()
                    .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
                    .ForMember(dest => dest.NextExpectedVersion, opt => opt.MapFrom(src => src.NextExpectedVersion))
                    .ForMember(dest => dest.LogPosition, opt => opt.MapFrom(src => src.LogPosition));


                cfg.CreateMap<CreatedOrderItemEvent, OrderItem>()
                    .ForMember(dest => dest.Order, opt => opt.Ignore())
                    .ForMember(dest => dest.NextExpectedVersion, opt => opt.MapFrom(src => src.NextExpectedVersion))
                    .ForMember(dest => dest.LogPosition, opt => opt.MapFrom(src => src.LogPosition));

                cfg.CreateMap<EventModel, CreatedOrderEvent>()
                    .ForMember(dest => dest.CustomerId,
                        opt => opt.MapFrom(src => ((CreatedOrderEvent) src.Data).CustomerId))
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ((CreatedOrderEvent) src.Data).Id))
                    .ForMember(dest => dest.ItemsCount,
                        opt => opt.MapFrom(src => ((CreatedOrderEvent) src.Data).ItemsCount))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ((CreatedOrderEvent) src.Data).Status))
                    .ForMember(dest => dest.Total, opt => opt.MapFrom(src => ((CreatedOrderEvent) src.Data).Total))
                    .ForMember(dest => dest.Vat, opt => opt.MapFrom(src => ((CreatedOrderEvent) src.Data).Vat))
                    .ForMember(dest => dest.NextExpectedVersion, opt => opt.MapFrom(src => src.NextExpectedVersion))
                    .ForMember(dest => dest.LogPosition, opt => opt.MapFrom(src => src.LogPosition));

                cfg.CreateMap<EventModel, CreatedOrderItemEvent>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ((CreatedOrderItemEvent)src.Data).Id))
                    .ForMember(dest => dest.OrderId,
                        opt => opt.MapFrom(src => ((CreatedOrderItemEvent)src.Data).OrderId))
                    .ForMember(dest => dest.Price, opt => opt.MapFrom(src => ((CreatedOrderItemEvent)src.Data).Price))
                    .ForMember(dest => dest.Quantity,
                        opt => opt.MapFrom(src => ((CreatedOrderItemEvent)src.Data).Quantity))
                    .ForMember(dest => dest.NextExpectedVersion, opt => opt.MapFrom(src => src.NextExpectedVersion))
                    .ForMember(dest => dest.LogPosition, opt => opt.MapFrom(src => src.LogPosition));

                cfg.CreateMap<EventModel, PlacedOrderEvent>()
                    .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => ((PlacedOrderEvent)src.Data).OrderId))
                    .ForMember(dest => dest.NextExpectedVersion, opt => opt.MapFrom(src => src.NextExpectedVersion))
                    .ForMember(dest => dest.LogPosition, opt => opt.MapFrom(src => src.LogPosition));

                cfg.CreateMap<EventModel, CancelOrderEvent>()
                    .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => ((CancelOrderEvent)src.Data).OrderId))
                    .ForMember(dest => dest.NextExpectedVersion, opt => opt.MapFrom(src => src.NextExpectedVersion))
                    .ForMember(dest => dest.LogPosition, opt => opt.MapFrom(src => src.LogPosition));

                cfg.CreateMap<CreateCustomerEvent, Customer>()
                    .ForMember(dest => dest.Orders, opt => opt.Ignore());

                cfg.CreateMap<EventModel, CreateCustomerEvent>()
                    .ForMember(dest => dest.CreatedDate,
                        opt => opt.MapFrom(src => ((CreateCustomerEvent) src.Data).CreatedDate))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => ((CreateCustomerEvent) src.Data).Email))
                    .ForMember(dest => dest.FullName,
                        opt => opt.MapFrom(src => ((CreateCustomerEvent) src.Data).FullName))
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ((CreateCustomerEvent) src.Data).Id))
                    .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => ((CreateCustomerEvent) src.Data).Phone))
                    .ForMember(dest => dest.NextExpectedVersion, opt => opt.MapFrom(src => src.NextExpectedVersion))
                    .ForMember(dest => dest.LogPosition, opt => opt.MapFrom(src => src.LogPosition));
            });

            Mapper = mappingConfig.CreateMapper();
        }
    }
}