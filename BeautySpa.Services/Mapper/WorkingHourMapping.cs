using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.WorkingHourModelViews;

namespace BeautySpa.Services.Mapper
{
    public class WorkingHourMapping : Profile
    {
        public WorkingHourMapping()
        {
            CreateMap<WorkingHour, GETWorkingHourModelViews>().ReverseMap();
            CreateMap<WorkingHour, POSTWorkingHourModelViews>().ReverseMap();
            CreateMap<WorkingHour, PUTWorkingHourModelViews>().ReverseMap();
            CreateMap<POSTWorkingHourModelViews, WorkingHour>().ReverseMap();
        }
    }
}
