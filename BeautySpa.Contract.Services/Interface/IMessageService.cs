using BeautySpa.Core.Base;
using BeautySpa.ModelViews.MessageModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IMessageService
    {
        Task<BasePaginatedList<GETMessageModelViews>> GetAllAsync(int pageNumber, int pageSize);
        Task<GETMessageModelViews> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(POSTMessageModelViews model);
        Task UpdateAsync(PUTMessageModelViews model);
        Task DeleteAsync(Guid id);
    }
}
