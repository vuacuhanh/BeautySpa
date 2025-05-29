using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.PromotionAdminModelView;

namespace BeautySpa.Services.Mapper
{
    public class PromotionAdminMapping : Profile
    {
        public PromotionAdminMapping()
        {
            CreateMap<PromotionAdmin, GETPromotionAdminModelView>()
                .ForMember(dest => dest.RankIds, opt => opt.MapFrom(src =>
                    src.PromotionAdminRanks.Select(par => par.RankId)));

            CreateMap<POSTPromotionAdminModelView, PromotionAdmin>();
            CreateMap<PUTPromotionAdminModelView, PromotionAdmin>();
        }
    }
}