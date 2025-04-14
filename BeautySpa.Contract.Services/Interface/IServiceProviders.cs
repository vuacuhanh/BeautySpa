using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceProviderModelViews;


namespace BeautySpa.Contract.Services.Interface
{
    public interface IServiceProviders
    {
        Task<Guid> CreateAsync(POSTServiceProviderModelViews model);
        Task<BasePaginatedList<GETServiceProviderModelViews>> GetAllAsync(int pageNumber, int pageSize);
        Task<GETServiceProviderModelViews> GetByIdAsync(Guid id);
        Task UpdateAsync(PUTServiceProviderModelViews model);
        Task DeleteAsync(Guid id);
    }
}
