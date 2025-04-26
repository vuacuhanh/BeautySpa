using BeautySpa.Core.Base;
using BeautySpa.ModelViews.RoleModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IRoles
    {
        Task<BasePaginatedList<GETRoleModelViews>> GetAllAsync(int pageNumber, int pageSize);
        Task<GETRoleModelViews> GetByIdAsync(Guid roleid);
        Task<Guid> CreateAsync(POSTRoleModelViews rolemodel);
        Task UpdateAsync(PUTRoleModelViews rolemodel);
        Task DeleteAsync(Guid roleid);
    }
}
