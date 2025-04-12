using BeautySpa.Core.Base;
using BeautySpa.ModelViews.RoleModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IRoles
    {
        Task<BasePaginatedList<GETRoleModelViews>> GetAllAsync(int pageNumber, int pageSize);
        Task<GETRoleModelViews> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(POSTRoleModelViews model);
        Task UpdateAsync(PUTRoleModelViews model);
    }
}
