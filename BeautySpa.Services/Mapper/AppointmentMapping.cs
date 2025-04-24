using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.AppointmentModelViews;

namespace BeautySpa.Services.Mapper
{
    public class AppointmentMapping : Profile
    {
        public AppointmentMapping()
        {
            CreateMap<Appointment, GETAppointmentModelViews>();

            CreateMap<POSTAppointmentModelViews, Appointment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore());

            CreateMap<PUTAppointmentModelViews, Appointment>()
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore());
        }
    }
}
