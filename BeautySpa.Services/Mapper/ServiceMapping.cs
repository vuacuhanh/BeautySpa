using AutoMapper;
using BeautySpa.ModelViews.ServiceModelViews;
using BeautySpa.ModelViews.ServiceImageModelViews;
using BeautySpa.Contract.Repositories.Entity;

namespace BeautySpa.Services.Mapper
{
    public class ServiceMapping : Profile
    {
        public ServiceMapping()
        {
            // Mapping Entity -> GET View
            CreateMap<BeautySpa.Contract.Repositories.Entity.Service, GETServiceModelViews>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.ServiceCategoryId))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.ServiceCategory!.CategoryName))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.DurationMinutes))
                .ReverseMap();

            // Mapping POST View -> Entity
            CreateMap<POSTServiceModelViews, BeautySpa.Contract.Repositories.Entity.Service>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceCategoryId, opt => opt.MapFrom(src => src.CategoryId));

            // Mapping PUT View -> Entity
            CreateMap<PUTServiceModelViews, BeautySpa.Contract.Repositories.Entity.Service>()
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceCategoryId, opt => opt.MapFrom(src => src.CategoryId));

            // Mapping ServiceImage
            CreateMap<ServiceImage, GETServiceImageModelViews>();

            CreateMap<POSTServiceImageModelViews, ServiceImage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore());
        }
    }
}