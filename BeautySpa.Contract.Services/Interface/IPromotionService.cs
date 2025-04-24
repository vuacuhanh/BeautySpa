using BeautySpa.Core.Base;
using BeautySpa.ModelViews.PromotionModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IPromotionService
    {
        Task<BasePaginatedList<GETPromotionModelViews>> GetAllAsync(int pageNumber, int pageSize);
        Task<GETPromotionModelViews> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(POSTPromotionModelViews model);
        Task UpdateAsync(PUTPromotionModelViews model);
        Task DeleteAsync(Guid id);
    }
}
