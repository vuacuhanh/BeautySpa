using BeautySpa.Core.Base;
using BeautySpa.ModelViews.PaymentModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IPaymentService
    {
        Task<BasePaginatedList<GETPaymentModelViews>> GetAllAsync(int pageNumber, int pageSize);
        Task<GETPaymentModelViews> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(POSTPaymentModelViews model);
        Task UpdateAsync(PUTPaymentModelViews model);
        Task DeleteAsync(Guid id);
    }
}
