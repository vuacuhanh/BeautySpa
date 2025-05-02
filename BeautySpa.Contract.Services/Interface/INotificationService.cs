using BeautySpa.Core.Base;
using BeautySpa.ModelViews.NotificationModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface INotificationService
    {
        Task<BaseResponseModel<List<GETNotificationModelView>>> GetAllByUserIdAsync(Guid userId);
        Task<BaseResponseModel<string>> CreateAsync(POSTNotificationModelView model);
        Task<BaseResponseModel<string>> MarkAsReadAsync(Guid notificationId);
        Task<BaseResponseModel<int>> GetUnreadCountAsync(Guid userId);
        Task<BaseResponseModel<string>> DeleteAsync(Guid notificationId);
    }
}
