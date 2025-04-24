using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.FavoriteModelViews;

namespace BeautySpa.Services.Mapper
{
    public class FavoriteMapping : Profile
    {
        public FavoriteMapping()
        {
            // Entity -> GET View
            CreateMap<Favorite, GETFavoriteModelViews>();

            // POST View -> Entity
            CreateMap<POSTFavoriteModelViews, Favorite>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id sẽ được tạo mới trong service
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Provider, opt => opt.Ignore());

            // PUT View -> Entity
            CreateMap<PUTFavoriteModelViews, Favorite>()
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore()) // Không cho sửa CreatedTime
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Provider, opt => opt.Ignore());
        }
    }
}
