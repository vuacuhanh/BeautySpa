using BeautySpa.Core.Base;
using BeautySpa.ModelViews.StaffModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IStaff
    {
        Task<BaseResponseModel<BasePaginatedList<GETStaffModelView>>> GetAllAsync(int pageNumber, int pageSize, Guid? providerId);
        Task<BaseResponseModel<GETStaffModelView>> GetByIdAsync(Guid id);
        Task<BaseResponseModel<Guid>> CreateAsync(POSTStaffModelView model);
        Task<BaseResponseModel<string>> UpdateAsync(PUTStaffModelView model);
        Task<BaseResponseModel<string>> DeleteAsync(Guid id);
    }
}