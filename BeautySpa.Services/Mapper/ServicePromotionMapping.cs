using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.ServicePromotionModelView;

namespace BeautySpa.Services.Mapper
{
    public class ServicePromotionMapping : Profile
    {
        public ServicePromotionMapping()
        {
            CreateMap<ServicePromotion, GETServicePromotionModelView>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service!.ServiceName));

            CreateMap<POSTServicePromotionModelView, ServicePromotion>();
            CreateMap<PUTServicePromotionModelView, ServicePromotion>();
        }
    }
}