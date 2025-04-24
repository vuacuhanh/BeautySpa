using BeautySpa.Core.Base;
using BeautySpa.ModelViews.AppointmentModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IAppointmentService
    {
        Task<BasePaginatedList<GETAppointmentModelViews>> GetAllAsync(int pageNumber, int pageSize);
        Task<GETAppointmentModelViews> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(POSTAppointmentModelViews model);
        Task UpdateAsync(PUTAppointmentModelViews model);
        Task DeleteAsync(Guid id);
    }
}
