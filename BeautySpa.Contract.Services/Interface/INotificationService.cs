using BeautySpa.Core.Base;
using BeautySpa.ModelViews.NotificationModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface INotificationService
    {
        Task<BasePaginatedList<GETNotificationModelViews>> GetAllAsync(int pageNumber, int pageSize);
        Task<GETNotificationModelViews> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(POSTNotificationModelViews model);
        Task UpdateAsync(PUTNotificationModelViews model);
        Task DeleteAsync(Guid id);
    }
}
