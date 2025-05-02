using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.NotificationModelViews;

namespace BeautySpa.Services.Mapper
{
    public class NotificationMapping : Profile
    {
        public NotificationMapping()
        {
            CreateMap<Notification, GETNotificationModelView>();
            CreateMap<POSTNotificationModelView, Notification>();
        }
    }
}
