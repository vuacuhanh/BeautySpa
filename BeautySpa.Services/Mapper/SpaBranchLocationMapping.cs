using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.LocationModelViews;

namespace BeautySpa.Services.Mapper
{
    public class SpaBranchLocationMapping : Profile
    {
        public SpaBranchLocationMapping()
        {
            CreateMap<SpaBranchLocation, GETSpaBranchLocationModelView>()
                .ForMember(dest => dest.FullAddress,
                    opt => opt.MapFrom(src => $"{src.Street}, {src.District}, {src.City}, {src.Country}"));

            CreateMap<POSTSpaBranchLocationModelView, SpaBranchLocation>();
            CreateMap<PUTSpaBranchLocationModelView, SpaBranchLocation>();
        }
    }
}