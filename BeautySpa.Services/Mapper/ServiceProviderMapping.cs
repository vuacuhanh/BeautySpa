using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.ServiceProviderModelViews;

namespace BeautySpa.Services.Mapper
{
    public class ServiceProviderMapping : Profile
    {
        public ServiceProviderMapping()
        {
            CreateMap<ServiceProvider, GETServiceProviderModelViews>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ServiceImages))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src =>
                    src.ServiceProviderCategories.Select(spc => spc.ServiceCategory!.CategoryName).ToList()));

            CreateMap<POSTServiceProviderModelViews, ServiceProvider>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.AverageRating, opt => opt.Ignore())
                .ForMember(dest => dest.TotalReviews, opt => opt.Ignore())
                .ForMember(dest => dest.IsApproved, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.ProviderId, opt => opt.MapFrom(src => src.UserId));

            CreateMap<PUTServiceProviderModelViews, ServiceProvider>()
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.ProviderId, opt => opt.Ignore()) 
                .ForMember(dest => dest.Email, opt => opt.Ignore())     
                .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore());
         }
    }
}
