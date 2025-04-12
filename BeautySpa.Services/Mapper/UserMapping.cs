using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.UserModelViews;

namespace BeautySpa.Services.Mapper
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            // ApplicationUsers -> GETUserModelViews
            CreateMap<ApplicationUsers, GETUserModelViews>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.UserInfor != null ? src.UserInfor.FullName : string.Empty))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.UserInfor != null ? src.UserInfor.Address : null))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.UserInfor != null ? src.UserInfor.DayofBirth ?? DateTime.MinValue : DateTime.MinValue))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status == "active"));

            // UserInfor -> GETUserInfoModelView
            CreateMap<UserInfor, GETUserInfoModelView>();

            // ApplicationUsers -> GETUserInfoforcustomerModelView
            CreateMap<ApplicationUsers, GETUserInfoforcustomerModelView>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.UserInfor != null ? src.UserInfor.FullName : string.Empty))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.UserInfor != null ? src.UserInfor.Address : null))
                .ForMember(dest => dest.DayofBirth, opt => opt.MapFrom(src => src.UserInfor != null ? src.UserInfor.DayofBirth : null))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.UserInfor != null ? src.UserInfor.AvatarUrl : null))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.UserInfor != null ? src.UserInfor.Gender : null))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status == "active"));

            // PUTUserModelViews -> UserInfor
            CreateMap<PUTUserModelViews, UserInfor>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.DayofBirth, opt => opt.MapFrom(src => src.DateOfBirth));

            // PUTuserforcustomer -> UserInfor
            CreateMap<PUTuserforcustomer, UserInfor>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.UserInfor.FullName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.UserInfor.Address))
                .ForMember(dest => dest.DayofBirth, opt => opt.MapFrom(src => src.UserInfor.DayofBirth))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.UserInfor.AvatarUrl))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.UserInfor.Gender));

            // PUTuserforcustomer -> ApplicationUsers
            CreateMap<PUTuserforcustomer, ApplicationUsers>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

        }
    }
}