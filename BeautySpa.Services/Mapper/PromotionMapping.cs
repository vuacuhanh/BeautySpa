using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.PromotionModelViews;

namespace BeautySpa.Services.Mapper
{
    public class PromotionMapping : Profile
    {
        public PromotionMapping()
        {
            CreateMap<Promotion, GETPromotionModelView>()
                .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.Provider!.UserInfor!.FullName))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => TimeHelper.ConvertToUtcPlus7(src.StartDate)))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => TimeHelper.ConvertToUtcPlus7(src.EndDate)));
            CreateMap<POSTPromotionModelView, Promotion>();
            CreateMap<PUTPromotionModelView, Promotion>();
        }
    }
}