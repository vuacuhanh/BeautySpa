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
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status == "active"))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // UserInfor => GETUserInfoModelView
            CreateMap<UserInfor, GETUserInfoModelView>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.AddressDetail, opt => opt.MapFrom(src => src.AddressDetail))
                .ForMember(dest => dest.ProvinceId, opt => opt.MapFrom(src => src.ProvinceId))
                .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.ProvinceName))
                .ForMember(dest => dest.DistrictId, opt => opt.MapFrom(src => src.DistrictId))
                .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.DistrictName))
                .ForMember(dest => dest.DayofBirth, opt => opt.MapFrom(src => src.DayofBirth))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // PUTUserModelViews => UserInfor
            CreateMap<PUTUserModelViews, UserInfor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id tránh lỗi primary key
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.AddressDetail, opt => opt.MapFrom(src => src.AddressDetail))
                .ForMember(dest => dest.ProvinceId, opt => opt.MapFrom(src => src.ProvinceId))
                .ForMember(dest => dest.DistrictId, opt => opt.MapFrom(src => src.DistrictId))
                .ForMember(dest => dest.DayofBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
                

            // PUTuserforcustomer => UserInfor
            CreateMap<PUTuserforcustomer, UserInfor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id tránh lỗi primary key
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.UserInfor.FullName))
                //.ForMember(dest => dest.AddressDetail, opt => opt.MapFrom(src => src.AddressDetail))
                //.ForMember(dest => dest.ProvinceId, opt => opt.MapFrom(src => src.ProvinceId))
                //.ForMember(dest => dest.DistrictId, opt => opt.MapFrom(src => src.DistrictId))
                .ForMember(dest => dest.DayofBirth, opt => opt.MapFrom(src => src.UserInfor.DateOfBirth))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.UserInfor.AvatarUrl))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.UserInfor.Gender))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // PUTuserforcustomer => ApplicationUsers
            CreateMap<PUTuserforcustomer, ApplicationUsers>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
