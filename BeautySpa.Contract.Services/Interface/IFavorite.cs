using BeautySpa.Core.Base;
using BeautySpa.ModelViews.FavoriteModelViews;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IFavoriteService
    {
        /// <summary>
        /// Lấy tất cả danh sách yêu thích (có phân trang)
        /// </summary>
        Task<BasePaginatedList<GETFavoriteModelViews>> GetAllAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Lấy chi tiết yêu thích theo ID
        /// </summary>
        Task<GETFavoriteModelViews> GetByIdAsync(Guid id);

        /// <summary>
        /// Tạo mới một mục yêu thích
        /// </summary>
        Task<Guid> CreateAsync(POSTFavoriteModelViews model);

        /// <summary>
        /// Cập nhật một mục yêu thích
        /// </summary>
        Task UpdateAsync(PUTFavoriteModelViews model);

        /// <summary>
        /// Xóa một mục yêu thích theo ID
        /// </summary>
        Task DeleteAsync(Guid id);
    }
}
