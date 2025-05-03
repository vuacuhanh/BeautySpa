using BeautySpa.Core.Base;
using BeautySpa.ModelViews.RankModelViews;

public interface IRankService
{
    Task<BaseResponseModel<BasePaginatedList<GETRankModelView>>> GetAllAsync(int pageNumber, int pageSize);
    Task<BaseResponseModel<GETRankModelView>> GetByIdAsync(Guid id);
    Task<BaseResponseModel<Guid>> CreateAsync(POSTRankModelView model);
    Task<BaseResponseModel<string>> UpdateAsync(PUTRankModelView model);
    Task<BaseResponseModel<string>> DeleteAsync(Guid id);
}
