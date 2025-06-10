using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ReviewModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IReviewService
    {
        Task<BasePaginatedList<GETReviewModelViews>> GetAllAsync(int pageNumber, int pageSize);
        Task<GETReviewModelViews> GetByIdAsync(Guid id);
        Task<BaseResponseModel<Guid>> CreateAsync(POSTReviewModelViews model);
        Task UpdateAsync(PUTReviewModelViews model);
        Task DeleteAsync(Guid id);
    }
}
