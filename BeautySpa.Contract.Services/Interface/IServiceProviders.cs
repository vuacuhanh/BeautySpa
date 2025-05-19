using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceProviderModelViews;


namespace BeautySpa.Contract.Services.Interface
{
    public interface IServiceProviders
    {
        Task<BaseResponseModel<BasePaginatedList<GETServiceProviderModelViews>>> GetAllAsync(int pageNumber, int pageSize);
        Task<BaseResponseModel<GETServiceProviderModelViews>> GetByIdAsync(Guid id);
        Task<BaseResponseModel<Guid>> CreateAsync(POSTServiceProviderModelViews model);
        Task<BaseResponseModel<string>> UpdateAsync(PUTServiceProviderModelViews model);
        Task<BaseResponseModel<string>> DeleteAsync(Guid id);
        Task<BaseResponseModel<List<GETServiceProviderModelViews>>> GetByCategory(Guid categoryId);

    }
}
