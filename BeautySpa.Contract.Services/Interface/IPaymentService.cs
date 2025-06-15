using BeautySpa.Core.Base;
using BeautySpa.ModelViews.PaymentModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IPaymentService
    {
        Task<BaseResponseModel<PaymentResponse>> CreateDepositAsync(POSTPaymentModelView model);
        Task<BaseResponseModel<string>> RefundDepositAsync(RefundPaymentModelView model);
        Task<BaseResponseModel<GETPaymentModelView>> GetByAppointmentIdAsync(Guid appointmentId);
    }
}
