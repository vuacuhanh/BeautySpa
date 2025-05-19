using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceImageModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IServiceImages
    {
        Task<BaseResponseModel<string>> CreateMultipleAsync(POSTServiceImageModelViews model);
        Task<BaseResponseModel<BasePaginatedList<GETServiceImageModelViews>>> GetPagedByProviderIdAsync(Guid providerId, int page, int size);
        Task<BaseResponseModel<List<GETServiceImageModelViews>>> GetByProviderIdAsync(Guid providerId);
        Task<BaseResponseModel<string>> DeleteAsync(Guid imageId);
    }
}