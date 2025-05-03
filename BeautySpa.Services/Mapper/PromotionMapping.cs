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
            CreateMap<POSTPromotionModelViews, Promotion>();
            CreateMap<PUTPromotionModelViews, Promotion>();
        }
    }
}