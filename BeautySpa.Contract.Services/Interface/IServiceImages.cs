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
        Task<BaseResponseModel<Guid>> CreateAsync(POSTServiceImageModelViews model);
        Task<BaseResponseModel<BasePaginatedList<GETServiceImageModelViews>>> GetAllAsync(int pageNumber, int pageSize);
        Task<BaseResponseModel<GETServiceImageModelViews>> GetByIdAsync(Guid id);
        Task<BaseResponseModel<string>> UpdateAsync(PUTServiceImageModelViews model);
        Task<BaseResponseModel<string>> DeleteAsync(Guid id);
        Task<BaseResponseModel<string>> SetPrimaryImageAsync(Guid imageId);
    }
}
