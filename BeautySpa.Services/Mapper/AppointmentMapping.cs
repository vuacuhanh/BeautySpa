using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.AppointmentModelViews;

namespace BeautySpa.Services.Mapper
{
    public class AppointmentMapping : Profile
    {
        public AppointmentMapping()
        {
            CreateMap<Appointment, GETAppointmentModelView>()
                .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.AppointmentServices));

            CreateMap<AppointmentService, AppointmentServiceDetail>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service!.ServiceName));

            CreateMap<POSTAppointmentModelView, Appointment>();
            CreateMap<PUTAppointmentModelView, Appointment>();
        }
    }
}
