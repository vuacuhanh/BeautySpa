using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceImageModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IServiceImages
    {
        Task<Guid> CreateAsync(POSTServiceImageModelViews model);
        Task<BasePaginatedList<GETServiceImageModelViews>> GetAllAsync(int pageNumber, int pageSize);
        Task<GETServiceImageModelViews> GetByIdAsync(Guid id);
        Task UpdateAsync(PUTServiceImageModelViews model);
        Task DeleteAsync(Guid id);
    }
}
