using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.ServiceProviderModelViews;

namespace BeautySpa.Services.Mapper
{
    public class ServiceProviderMapping : Profile
    {
        public ServiceProviderMapping()
        {
            CreateMap<ServiceProvider, GETServiceProviderModelViews>();
            CreateMap<POSTServiceProviderModelViews, ServiceProvider>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.AverageRating, opt => opt.Ignore())
                .ForMember(dest => dest.TotalReviews, opt => opt.Ignore())
                .ForMember(dest => dest.IsApproved, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());
            CreateMap<PUTServiceProviderModelViews, ServiceProvider>()
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore());
        }
    }
}
