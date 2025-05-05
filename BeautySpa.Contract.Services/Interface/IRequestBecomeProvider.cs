using BeautySpa.Core.Base;
using BeautySpa.ModelViews.RequestBecomeProviderModelView;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IRequestBecomeProvider
    {
        Task<BaseResponseModel<Guid>> CreateRequestAsync(POSTRequestBecomeProviderModelView model);
        Task<BaseResponseModel<BasePaginatedList<GETRequestBecomeProviderModelView>>> GetAllAsync(string? requestStatus, int pageNumber, int pageSize);
        Task<BaseResponseModel<string>> ApproveRequestAsync(Guid requestId);
        Task<BaseResponseModel<string>> RejectRequestAsync(Guid requestId, string reason);
    }
}
