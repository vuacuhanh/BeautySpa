using BeautySpa.Core.Base;
using BeautySpa.ModelViews.WorkingHourModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IWorkingHourService
    {
        Task<BasePaginatedList<GETWorkingHourModelViews>> GetAllAsync(int pageNumber, int pageSize);
        Task<GETWorkingHourModelViews> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(POSTWorkingHourModelViews model);
        Task UpdateAsync(PUTWorkingHourModelViews model);
        Task DeleteAsync(Guid id);
    }
}
