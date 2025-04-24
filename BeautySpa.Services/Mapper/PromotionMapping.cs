using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.PromotionModelViews;

namespace BeautySpa.Services.Mapper
{
    public class PromotionMapping : Profile
    {
        public PromotionMapping()
        {
            CreateMap<Promotion, GETPromotionModelViews>();

            CreateMap<POSTPromotionModelViews, Promotion>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore());

            CreateMap<PUTPromotionModelViews, Promotion>()
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore());
        }
    }
}
