using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.ServiceImageModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
