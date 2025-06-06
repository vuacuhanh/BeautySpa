using BeautySpa.Core.Base;
using BeautySpa.ModelViews.MemberShipModelViews.FavoriteModelViews;
using BeautySpa.ModelViews.ServiceProviderModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IFavoriteService
    {
        Task<BaseResponseModel<string>> LikeOrUnlikeAsync(Guid providerId);
        Task<BaseResponseModel<bool>> IsFavoriteAsync(Guid customerId, Guid providerId);
        Task<BaseResponseModel<List<GETFavoriteModelViews>>> GetFavoritesByProviderAsync(Guid providerId);
        Task<BaseResponseModel<List<GETServiceProviderModelViews>>> GetFavoritesByCustomerAsync(Guid customerId);

    }
}
