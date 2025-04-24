using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.WorkingHourModelViews;

namespace BeautySpa.Services.Mapper
{
    public class WorkingHourMapping : Profile
    {
        public WorkingHourMapping()
        {
            CreateMap<WorkingHour, GETWorkingHourModelViews>();

            CreateMap<POSTWorkingHourModelViews, WorkingHour>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore());

            CreateMap<PUTWorkingHourModelViews, WorkingHour>()
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore());
        }
    }
}
