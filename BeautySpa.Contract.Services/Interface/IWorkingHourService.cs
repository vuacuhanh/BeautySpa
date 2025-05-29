using BeautySpa.Core.Base;
using BeautySpa.ModelViews.WorkingHourModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IWorkingHourService
    {
        Task<BaseResponseModel<BasePaginatedList<GETWorkingHourModelViews>>> GetAllAsync(int pageNumber, int pageSize);
        Task<BaseResponseModel<GETWorkingHourModelViews>> GetByIdAsync(Guid id);
        Task<BaseResponseModel<Guid>> CreateAsync(POSTWorkingHourModelViews model);
        Task<BaseResponseModel<string>> UpdateAsync(PUTWorkingHourModelViews model);
        Task<BaseResponseModel<string>> DeleteAsync(Guid id);
    }
}
