using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.ServiceCategoryModelViews;
using BeautySpa.ModelViews.ServiceImageModelViews;
using BeautySpa.ModelViews.ServiceModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
