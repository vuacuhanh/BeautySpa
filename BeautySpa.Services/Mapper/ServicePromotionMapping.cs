using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.ServicePromotionModelView;

namespace BeautySpa.Services.Mapper
{
    public class ServicePromotionMapping : Profile
    {
        public ServicePromotionMapping()
        {
            CreateMap<ServicePromotion, GETServicePromotionModelView>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service!.ServiceName))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src =>
                    TimeHelper.ConvertToUtcPlus7(src.StartDate)))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src =>
                    TimeHelper.ConvertToUtcPlus7(src.EndDate)));

            CreateMap<POSTServicePromotionModelView, ServicePromotion>();
            CreateMap<PUTServicePromotionModelView, ServicePromotion>();
        }
    }
}