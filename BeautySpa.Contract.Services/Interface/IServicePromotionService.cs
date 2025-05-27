using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServicePromotionModelView;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IServicePromotionService
    {
        Task<BaseResponseModel<BasePaginatedList<GETServicePromotionModelView>>> GetAllAsync(int pageNumber, int pageSize);
        Task<BaseResponseModel<GETServicePromotionModelView>> GetByIdAsync(Guid id);
        Task<BaseResponseModel<List<GETServicePromotionModelView>>> GetAllByCurrentUserAsync();
        Task<BaseResponseModel<string>> CreateAsync(POSTServicePromotionModelView model);
        Task<BaseResponseModel<string>> UpdateAsync(PUTServicePromotionModelView model);
        Task<BaseResponseModel<string>> DeleteAsync(Guid id);
    }
}
