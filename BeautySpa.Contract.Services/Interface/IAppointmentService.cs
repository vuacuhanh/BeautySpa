using BeautySpa.Core.Base;
using BeautySpa.ModelViews.AppointmentModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IAppointmentService
    {
        Task<BaseResponseModel<AppointmentCreatedResult>> CreateAsync(POSTAppointmentModelView model);
        Task<BaseResponseModel<string>> UpdateAsync(PUTAppointmentModelView model);
        Task<BaseResponseModel<string>> UpdateStatusAsync(Guid appointmentId, string status);
        Task<BaseResponseModel<string>> DeleteAsync(Guid id);
        Task<BaseResponseModel<BasePaginatedList<GETAppointmentModelView>>> GetAllAsync(int pageNumber, int pageSize);
        Task<BaseResponseModel<GETAppointmentModelView>> GetByIdAsync(Guid id);
        Task<BaseResponseModel<string>> AutoCancelUnpaidAppointmentsAsync();
        Task<BaseResponseModel<string>> AutoNoShowAfter12HoursAsync();
        Task<BaseResponseModel<List<GETAppointmentModelView>>> GetByCurrentUserAsync();
        Task<BaseResponseModel<string>> CancelByUserAsync(Guid appointmentId);

    }
}
