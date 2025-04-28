using BeautySpa.Core.Base;
using BeautySpa.ModelViews.FavoriteModelViews;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IFavoriteService
    {
        Task<BasePaginatedList<GETFavoriteModelViews>> GetAllAsync(int pageNumber, int pageSize);
        Task<GETFavoriteModelViews> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(POSTFavoriteModelViews model);
        Task UpdateAsync(PUTFavoriteModelViews model);
        Task DeleteAsync(Guid id);
    }
}
