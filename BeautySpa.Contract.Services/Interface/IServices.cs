using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IServices
    {
        Task<Guid> CreateAsync(POSTServiceModelViews model);
        Task<BasePaginatedList<GETServiceModelViews>> GetAllAsync(int pageNumber, int pageSize);
        Task<GETServiceModelViews> GetByIdAsync(Guid id);
        Task UpdateAsync(PUTServiceModelViews model);
        Task DeleteAsync(Guid id);
    }
}
