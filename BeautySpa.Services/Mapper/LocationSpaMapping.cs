using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.LocationModelViews;

namespace BeautySpa.Services.Mapper
{
    public class LocationSpaMapping : Profile
    {
        public LocationSpaMapping()
        {
            // BranchLocationSpa
            CreateMap<BranchLocationSpa, GETBranchLocationModelView>();
            CreateMap<POSTBranchLocationModelView, BranchLocationSpa>();
            CreateMap<PUTBranchLocationModelView, BranchLocationSpa>();

            // LocationSpa
            CreateMap<LocationSpa, GETLocationSpaModelView>()
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch != null ? src.Branch.Name : string.Empty));

            CreateMap<POSTLocationSpaModelView, LocationSpa>();
            CreateMap<PUTLocationSpaModelView, LocationSpa>();
        }
    }
}
