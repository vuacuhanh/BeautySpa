
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.UserModelViews;

public interface IUsers
{
    Task<GETUserInfoModelView> GetByIdAsync(Guid id);
    Task<BasePaginatedList<GETUserModelViews>> GetAllAsync(int pageNumber, int pageSize);
    Task<GETUserInfoforcustomerModelView> GetCustomerInfoAsync(Guid id);
    Task UpdateAsync(PUTUserModelViews model);
    Task UpdateCustomerAsync(PUTuserforcustomer model);
    Task DeleteAsync(Guid id);

}