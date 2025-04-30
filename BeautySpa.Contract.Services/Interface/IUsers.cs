using BeautySpa.Core.Base;
using BeautySpa.ModelViews.UserModelViews;

public interface IUsers
{
    Task<BaseResponseModel<GETUserInfoModelView>> GetByIdAsync(Guid id);
    Task<BaseResponseModel<BasePaginatedList<GETUserModelViews>>> GetAllAsync(int pageNumber, int pageSize);
    Task<BaseResponseModel<BasePaginatedList<GETUserModelViews>>> GetCustomerAsync(int pageNumber, int pageSize);
    Task<BaseResponseModel<string>> UpdateAsync(PUTUserModelViews model);
    Task<BaseResponseModel<string>> UpdateCustomerAsync(PUTuserforcustomer model);
    Task<BaseResponseModel<string>> DeleteAsync(Guid id);
}
