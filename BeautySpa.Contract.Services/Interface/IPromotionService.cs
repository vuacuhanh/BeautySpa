using BeautySpa.Core.Base;
using BeautySpa.ModelViews.PromotionModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IPromotionService
    {
        Task<BaseResponseModel<BasePaginatedList<GETPromotionModelViews>>> GetAllAsync(int pageNumber, int pageSize);
        Task<BaseResponseModel<GETPromotionModelViews>> GetByIdAsync(Guid id);
        Task<BaseResponseModel<Guid>> CreateAsync(POSTPromotionModelViews model);
        Task<BaseResponseModel<string>> UpdateAsync(PUTPromotionModelViews model);
        Task<BaseResponseModel<string>> DeleteAsync(Guid id);
    }
}
