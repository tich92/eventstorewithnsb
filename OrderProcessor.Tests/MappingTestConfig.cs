using AutoMapper;
using Contracts.Events;
using EventStoreContext;
using EventStoreContext.Models;

namespace OrderProcessor.Tests
{ 
    public class MappingTestConfig
    {
        public IMapper Mapper { get; set; }

        public MappingTestConfig()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EventModel, CreatedOrderEvent>()
                    .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => ((CreatedOrderEvent)src.Data).CustomerId))
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ((CreatedOrderEvent)src.Data).Id))
                    .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => ((CreatedOrderEvent)src.Data).ItemsCount))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ((CreatedOrderEvent)src.Data).Status))
                    .ForMember(dest => dest.Total, opt => opt.MapFrom(src => ((CreatedOrderEvent)src.Data).Total))
                    .ForMember(dest => dest.Vat, opt => opt.MapFrom(src => ((CreatedOrderEvent)src.Data).Vat))
                    .ForMember(dest => dest.NextExpectedVersion, opt => opt.MapFrom(src => src.EventNumber));

                cfg.CreateMap<EventModel, CreatedOrderItemEvent>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ((CreatedOrderItemEvent) src.Data).Id))
                    .ForMember(dest => dest.OrderId,
                        opt => opt.MapFrom(src => ((CreatedOrderItemEvent) src.Data).OrderId))
                    .ForMember(dest => dest.Price, opt => opt.MapFrom(src => ((CreatedOrderItemEvent) src.Data).Price))
                    .ForMember(dest => dest.Quantity,
                        opt => opt.MapFrom(src => ((CreatedOrderItemEvent) src.Data).Quantity))
                    .ForMember(dest => dest.NextExpectedVersion, opt => opt.MapFrom(src => src.EventNumber));

                cfg.CreateMap<EventModel, PlacedOrderEvent>()
                    .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => ((PlacedOrderEvent) src.Data).OrderId))
                    .ForMember(dest => dest.NextExpectedVersion, opt => opt.MapFrom(src => src.EventNumber));

                cfg.CreateMap<EventModel, CancelOrderEvent>()
                    .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => ((CancelOrderEvent)src.Data).OrderId))
                    .ForMember(dest => dest.NextExpectedVersion, opt => opt.MapFrom(src => src.EventNumber));
            });

            Mapper = config.CreateMapper();
        }
    }
}