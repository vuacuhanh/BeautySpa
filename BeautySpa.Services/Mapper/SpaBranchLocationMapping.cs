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
                .ForMember(dest => dest.WorkingHours, opt => opt.MapFrom(src =>
                    src.WorkingHours != null
                        ? src.WorkingHours.OrderBy(x => x.DayOfWeek).ToList()
                        : new List<WorkingHour>()));

            CreateMap<POSTSpaBranchLocationModelView, SpaBranchLocation>();
            CreateMap<PUTSpaBranchLocationModelView, SpaBranchLocation>();
        }
    }
}
