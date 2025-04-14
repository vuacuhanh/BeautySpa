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
        Task<Guid> CreateAsync(POSTServiceCategoryModelViews model);
        Task<BasePaginatedList<GETServiceCategoryModelViews>> GetAllAsync(int pageNumber, int pageSize);
        Task<GETServiceCategoryModelViews> GetByIdAsync(Guid id);
        Task UpdateAsync(PUTServiceCategoryModelViews model);
        Task DeleteAsync(Guid id);
    }
}
