using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.ServiceImageModelViews;

namespace BeautySpa.Services.Mapper
{
    public class ServiceImageMapping : Profile
    {
        public ServiceImageMapping()
        {
            CreateMap<ServiceImage, GETServiceImageModelViews>();
        }
    }
}
