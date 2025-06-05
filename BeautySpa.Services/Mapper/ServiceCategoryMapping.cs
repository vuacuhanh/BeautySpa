using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.ServiceCategoryModelViews;

namespace BeautySpa.Services.Mapper
{
    public class ServiceCategoryMapping : Profile
    {
        public ServiceCategoryMapping()
        {

            // Ánh xạ cho ServiceCategory
            CreateMap<ServiceCategory, GETServiceCategoryModelViews>();
            CreateMap<POSTServiceCategoryModelViews, ServiceCategory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.Services, opt => opt.Ignore());
            CreateMap<PUTServiceCategoryModelViews, ServiceCategory>()
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
                .ForMember(dest => dest.Services, opt => opt.Ignore());
        }
    }
}
