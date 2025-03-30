using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.AuthModelViews;


namespace BeautySpa.Services.Mapper
{
    public class AuthMapping : Profile
    {
        public AuthMapping()
        {
            // Map từ SignUpAuthModelView sang ApplicationUser
            CreateMap<SignUpAuthModelView, ApplicationUsers>()
                .ForMember(dest => dest.UserInfor, opt => opt.MapFrom(src => new UserInfor { FullName = src.FullName }));
        }
    }
}
