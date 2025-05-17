using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.LocationModelViews;

namespace BeautySpa.Services.Mapper
{
    public class SpaBranchLocationMapping : Profile
    {
        public SpaBranchLocationMapping()
        {
            CreateMap<SpaBranchLocation, GETSpaBranchLocationModelView>();
            CreateMap<POSTSpaBranchLocationModelView, SpaBranchLocation>();
            CreateMap<PUTSpaBranchLocationModelView, SpaBranchLocation>();
        }
    }
}