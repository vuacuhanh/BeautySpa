using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.PromotionModelViews;

namespace BeautySpa.Services.Mapper
{
    public class PromotionMapping : Profile
    {
        public PromotionMapping()
        {
            CreateMap<Promotion, GETPromotionModelView>()
                .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.Provider!.UserInfor!.FullName));

            CreateMap<POSTPromotionModelView, Promotion>();
            CreateMap<PUTPromotionModelView, Promotion>();
        }
    }
}