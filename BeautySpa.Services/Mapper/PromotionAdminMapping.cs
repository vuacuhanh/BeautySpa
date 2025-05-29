using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.PromotionAdminModelView;

namespace BeautySpa.Services.Mapper
{
    public class PromotionAdminMapping : Profile
    {
        public PromotionAdminMapping()
        {
            CreateMap<PromotionAdmin, GETPromotionAdminModelView>()
                .ForMember(dest => dest.RankIds, opt => opt.MapFrom(src =>
                    src.PromotionAdminRanks.Select(par => par.RankId)))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src =>
                    TimeHelper.ConvertToUtcPlus7(src.StartDate)))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src =>
                    TimeHelper.ConvertToUtcPlus7(src.EndDate)));

            CreateMap<POSTPromotionAdminModelView, PromotionAdmin>();
            CreateMap<PUTPromotionAdminModelView, PromotionAdmin>();
        }
    }
}