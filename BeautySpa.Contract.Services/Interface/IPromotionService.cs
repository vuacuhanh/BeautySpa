using BeautySpa.Core.Base;
using BeautySpa.ModelViews.PromotionModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IPromotionService
    {
        Task<BaseResponseModel<BasePaginatedList<GETPromotionModelView>>> GetAllAsync(int pageNumber, int pageSize);
        Task<BaseResponseModel<List<GETPromotionModelView>>> GetByProviderIdAsync(Guid providerId);
        Task<BaseResponseModel<GETPromotionModelView>> GetByIdAsync(Guid id);
        Task<BaseResponseModel<string>> CreateAsync(POSTPromotionModelView model);
        Task<BaseResponseModel<string>> UpdateAsync(PUTPromotionModelView model);
        Task<BaseResponseModel<string>> DeleteAsync(Guid id);
    }
}