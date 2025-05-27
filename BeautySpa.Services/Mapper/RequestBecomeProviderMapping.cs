using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.RequestBecomeProviderModelView;

namespace BeautySpa.Services.Mapper
{
    public class RequestBecomeProviderMapping : Profile
    {
        public RequestBecomeProviderMapping()
        {
            CreateMap<POSTRequestBecomeProviderModelView, RequestBecomeProvider>()
                .ForMember(dest => dest.ServiceCategoryIds, opt => opt.MapFrom(src => string.Join("|", src.ServiceCategoryIds)))
                .ForMember(dest => dest.DescriptionImages, opt => opt.MapFrom(src => string.Join("|", src.DescriptionImages!)))
                .ForMember(dest => dest.ProvinceId, opt => opt.MapFrom(src => src.ProvinceId!.ToString()))
                .ForMember(dest => dest.DistrictId, opt => opt.MapFrom(src => src.DistrictId!.ToString()))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.PostalCode));

            CreateMap<RequestBecomeProvider, GETRequestBecomeProviderModelView>()
                .ForMember(dest => dest.ServiceCategoryIds, opt => opt.MapFrom(src => src.ServiceCategoryIds!.Split('|', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList()))
                .ForMember(dest => dest.DescriptionImages, opt => opt.MapFrom(src => src.DescriptionImages!.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList()))
                .ForMember(dest => dest.ProvinceId, opt => opt.MapFrom(src => src.ProvinceId))
                .ForMember(dest => dest.DistrictId, opt => opt.MapFrom(src => src.DistrictId))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
                .ForMember(dest => dest.RequestStatus, opt => opt.MapFrom(src => src.RequestStatus));
            CreateMap<RegisterRequestBecomeProviderModelView, RequestBecomeProvider>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.ServiceCategoryIds, opt => opt.MapFrom(src => string.Join("|", src.ServiceCategoryIds)))
                .ForMember(dest => dest.ProvinceId, opt => opt.MapFrom(src => src.ProvinceId))
                .ForMember(dest => dest.DistrictId, opt => opt.MapFrom(src => src.DistrictId));
        }
    }
}
