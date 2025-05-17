using BeautySpa.Core.Base;
using BeautySpa.ModelViews.LocationModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface ISpaBranchLocationService
    {
        Task<BaseResponseModel<Guid>> CreateAsync(POSTSpaBranchLocationModelView model);
        Task<BaseResponseModel<string>> UpdateAsync(PUTSpaBranchLocationModelView model);
        Task<BaseResponseModel<string>> DeleteAsync(Guid id);
        Task<BaseResponseModel<GETSpaBranchLocationModelView>> GetByIdAsync(Guid id);
        Task<BaseResponseModel<BasePaginatedList<GETSpaBranchLocationModelView>>> GetAllAsync(int pageNumber, int pageSize);
        Task<BaseResponseModel<List<GETSpaBranchLocationModelView>>> GetByProviderAsync(Guid providerId);
    }
}