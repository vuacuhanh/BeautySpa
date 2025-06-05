using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.MemberShipModelViews;

namespace BeautySpa.Services.Mapper
{
    public class MemberShipMapping : Profile
    {
        public MemberShipMapping()
        {
            CreateMap<MemberShip, GETMemberShipModelView>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User!.UserInfor!.FullName))
                .ForMember(dest => dest.RankName, opt => opt.MapFrom(src => src.Rank != null ? src.Rank.Name : string.Empty))
                .ForMember(dest => dest.LastRankUpdate, opt => opt.MapFrom(src => src.LastUpdatedTime));

            CreateMap<POSTMemberShipModelView, MemberShip>()
                .ForMember(dest => dest.AccumulatedPoints, opt => opt.MapFrom(src => src.InitialPoints));
        }
    }
}
