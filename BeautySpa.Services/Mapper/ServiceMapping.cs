// ========================
// ServiceMapping.cs – Clean Mapping Configurations
// ========================
using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.ServiceModelViews;
using BeautySpa.ModelViews.ServiceImageModelViews;

namespace BeautySpa.Services.Mapper
{
    public class ServiceMapping : Profile
    {
        public ServiceMapping()
        {
            CreateMap<BeautySpa.Contract.Repositories.Entity.Service, GETServiceModelViews>();

            CreateMap<POSTServiceModelViews, BeautySpa.Contract.Repositories.Entity.Service>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceCategoryId, opt => opt.MapFrom(src => src.CategoryId)); 

            CreateMap<PUTServiceModelViews, BeautySpa.Contract.Repositories.Entity.Service>()
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceCategoryId, opt => opt.MapFrom(src => src.CategoryId)); 

            CreateMap<ServiceImage, GETServiceImageModelViews>();

            CreateMap<POSTServiceImageModelViews, ServiceImage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore());
        }
    }
}
