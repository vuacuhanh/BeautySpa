using BeautySpa.Core.Base;
using BeautySpa.ModelViews.MemberShipModelViews;
using BeautySpa.ModelViews.RankModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IMemberShipService
    {
        Task<BaseResponseModel<BasePaginatedList<GETMemberShipModelView>>> GetAllAsync(int pageNumber, int pageSize);
        Task<BaseResponseModel<GETMemberShipModelView>> GetByUserIdAsync(Guid userId);
        Task<BaseResponseModel<Guid>> CreateAsync(POSTMemberShipModelView model);
        Task<BaseResponseModel<string>> AddPointsAsync(PATCHMemberShipAddPointsModel model);
        Task<BaseResponseModel<string>> DeleteAsync(Guid id);
    }
}
