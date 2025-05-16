using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.MemberShipModelViews.FavoriteModelViews;

namespace BeautySpa.Services.Mapper
{
    public class FavoriteMapping : Profile
    {
        public FavoriteMapping()
        {
            CreateMap<Favorite, GETFavoriteModelViews>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.ProviderId, opt => opt.MapFrom(src => src.ProviderId))
                .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime));
        }
    }
}