using BeautySpa.Core.Base;
using BeautySpa.ModelViews.FavoriteModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IFavoriteService
    {
        Task<BaseResponseModel<string>> LikeOrUnlikeAsync(Guid customerId, Guid providerId);
        Task<BaseResponseModel<bool>> IsFavoriteAsync(Guid customerId, Guid providerId);
        Task<BaseResponseModel<List<GETFavoriteModelViews>>> GetFavoritesByProviderAsync(Guid providerId);
    }
}
