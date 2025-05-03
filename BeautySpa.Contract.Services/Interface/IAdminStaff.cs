using BeautySpa.Core.Base;
using BeautySpa.ModelViews.StaffAdminModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IAdminStaff
    {
        Task<BasePaginatedList<GETAdminStaffModelView>> GetAllAsync(int page, int size);
        Task<BaseResponseModel<GETAdminStaffModelView>> GetByIdAsync(Guid id);
        Task<BaseResponseModel<Guid>> CreateAsync(POSTAdminStaffModelView model);
        Task<BaseResponseModel<string>> UpdateAsync(PUTAdminStaffModelView model);
        Task<BaseResponseModel<string>> DeleteAsync(Guid id);
    }
}
