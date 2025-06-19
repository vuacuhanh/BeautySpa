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
                .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.AppointmentServices))
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Staff!.FullName))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.BranchLocation!.BranchName))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.BranchLocation!.Street))
                .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.BranchLocation!.DistrictName))
                .ForMember(dest => dest.IsReviewed, opt => opt.Ignore())
                .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.BranchLocation!.ProvinceName));

            CreateMap<AppointmentService, AppointmentServiceDetail>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service!.ServiceName));

            CreateMap<POSTAppointmentModelView, Appointment>()
                .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.StaffId));

            CreateMap<PUTAppointmentModelView, Appointment>()
                .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.StaffId));
        }
    }
}
