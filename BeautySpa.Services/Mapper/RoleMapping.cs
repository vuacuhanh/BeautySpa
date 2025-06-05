using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.RoleModelViews;

namespace BeautySpa.Services.Mapper
{
    public class RoleMapping : Profile
    {
        public RoleMapping()
        {
            // ApplicationRoles -> GETRoleModelView
            CreateMap<ApplicationRoles, GETRoleModelViews>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Name));

            // POSTRoleModelView -> ApplicationRoles
            CreateMap<POSTRoleModelViews, ApplicationRoles>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RoleName))
                .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => src.RoleName.ToUpperInvariant()))
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // ID được tạo trong service

            // PUTRoleModelView -> ApplicationRoles
            CreateMap<PUTRoleModelViews, ApplicationRoles>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RoleName))
                .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => src.RoleName.ToUpperInvariant()));
        }
    }
}