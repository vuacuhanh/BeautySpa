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
        Task<BaseResponseModel<Guid>> CreateAsync(POSTServiceModelViews model);
        Task<BaseResponseModel<BasePaginatedList<GETServiceModelViews>>> GetAllAsync(int pageNumber, int pageSize, bool isMineOnly);
        Task<BaseResponseModel<List<GETServiceModelViews>>> GetByProviderIdAsync(Guid providerId);
        Task<BaseResponseModel<GETServiceModelViews>> GetByIdAsync(Guid id);
        Task<BaseResponseModel<string>> UpdateAsync(PUTServiceModelViews model);
        Task<BaseResponseModel<string>> DeleteAsync(Guid id);
    }
}
