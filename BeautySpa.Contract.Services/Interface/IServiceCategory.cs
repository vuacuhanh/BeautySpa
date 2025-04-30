using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceCategoryModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IServiceCategory
    {
        Task<BaseResponseModel<Guid>> CreateAsync(POSTServiceCategoryModelViews model);
        Task<BaseResponseModel<BasePaginatedList<GETServiceCategoryModelViews>>> GetAllAsync(int pageNumber, int pageSize);
        Task<BaseResponseModel<GETServiceCategoryModelViews>> GetByIdAsync(Guid id);
        Task<BaseResponseModel<string>> UpdateAsync(PUTServiceCategoryModelViews model);
        Task<BaseResponseModel<string>> DeleteAsync(Guid id);
    }
}
