using BeautySpa.Core.Base;
using BeautySpa.ModelViews.AppointmentModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IAppointmentService
    {
        Task<BaseResponseModel<BasePaginatedList<GETAppointmentModelViews>>> GetAllAsync(int pageNumber, int pageSize);
        Task<BaseResponseModel<GETAppointmentModelViews>> GetByIdAsync(Guid id);
        Task<BaseResponseModel<Guid>> CreateAsync(POSTAppointmentModelViews model);
        Task<BaseResponseModel<string>> UpdateAsync(PUTAppointmentModelViews model);
        Task<BaseResponseModel<string>> DeleteAsync(Guid id);
    }
}
