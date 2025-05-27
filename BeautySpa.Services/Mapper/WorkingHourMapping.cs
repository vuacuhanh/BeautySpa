using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.LocationModelViews;

namespace BeautySpa.Services.Mappings
{
    public class SpaBranchLocationMapping : Profile
    {
        public SpaBranchLocationMapping()
        {
            CreateMap<SpaBranchLocation, GETSpaBranchLocationModelView>()
            .ForMember(dest => dest.WorkingHours, opt =>
                opt.MapFrom(src => (src.WorkingHours ?? new List<WorkingHour>())
                    .OrderBy(x => x.DayOfWeek).ToList()));  

            CreateMap<POSTSpaBranchLocationModelView, SpaBranchLocation>();
            CreateMap<PUTSpaBranchLocationModelView, SpaBranchLocation>();
        }
    }
}
