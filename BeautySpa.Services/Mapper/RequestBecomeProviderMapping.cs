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
             .ForMember(dest => dest.DescriptionImages, opt => opt.MapFrom(src => string.Join("|", src.DescriptionImages!)));

            CreateMap<RequestBecomeProvider, GETRequestBecomeProviderModelView>()
                .ForMember(dest => dest.ServiceCategoryIds, opt => opt.MapFrom(src => src.ServiceCategoryIds!.Split('|', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList()))
                .ForMember(dest => dest.DescriptionImages, opt => opt.MapFrom(src => src.DescriptionImages!.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList()))
                .ForMember(dest => dest.RequestStatus, opt => opt.MapFrom(src => src.RequestStatus));

        }
    }
}
