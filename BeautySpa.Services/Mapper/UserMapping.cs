using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.UserModelViews;

namespace BeautySpa.Services.Mapper
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            // ApplicationUsers => GETUserModelViews
            CreateMap<ApplicationUsers, GETUserModelViews>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.UserInfor != null ? src.UserInfor.FullName : string.Empty))
                .ForMember(dest => dest.AddressDetail, opt => opt.MapFrom(src => src.UserInfor!.AddressDetail))
                .ForMember(dest => dest.ProvinceId, opt => opt.MapFrom(src => src.UserInfor!.ProvinceId))
                .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.UserInfor!.ProvinceName))
                .ForMember(dest => dest.DistrictId, opt => opt.MapFrom(src => src.UserInfor!.DistrictId))
                .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.UserInfor!.DistrictName))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.UserInfor != null ? src.UserInfor.DayofBirth : DateTime.MinValue))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.UserInfor!.AvatarUrl))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.UserInfor!.Gender))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status == "active"))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));


            // PUTUserModelViews => UserInfor
            CreateMap<PUTUserModelViews, UserInfor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id tránh lỗi primary key
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.AddressDetail, opt => opt.MapFrom(src => src.AddressDetail))
                .ForMember(dest => dest.ProvinceId, opt => opt.MapFrom(src => src.ProvinceId))
                .ForMember(dest => dest.DistrictId, opt => opt.MapFrom(src => src.DistrictId))
                .ForMember(dest => dest.DayofBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
                
        }
    }
}
