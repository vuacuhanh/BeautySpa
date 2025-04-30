using BeautySpa.Core.Base;
using BeautySpa.ModelViews.RoleModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IRoles
    {
        Task<BaseResponseModel<BasePaginatedList<GETRoleModelViews>>> GetAllAsync(int pageNumber, int pageSize);
        Task<BaseResponseModel<GETRoleModelViews>> GetByIdAsync(Guid roleid);
        Task<BaseResponseModel<Guid>> CreateAsync(POSTRoleModelViews rolemodel);
        Task<BaseResponseModel<string>> UpdateAsync(PUTRoleModelViews rolemodel);
        Task<BaseResponseModel<string>> DeleteAsync(Guid roleid);
    }
}
