using BeautySpa.Core.Base;
using BeautySpa.ModelViews.PaymentModelViews;
using Newtonsoft.Json.Linq;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IPaymentService
    {
        Task<BaseResponseModel<PaymentResponse>> CreateDepositAsync(POSTPaymentModelView model);
        Task<BaseResponseModel<string>> RefundDepositAsync(RefundPaymentModelView model);
        Task<BaseResponseModel<GETPaymentModelView>> GetByAppointmentIdAsync(Guid appointmentId);
        Task<BaseResponseModel<string>> HandleVnpayIpnAsync(Dictionary<string, string> query);
        Task<BaseResponseModel<string>> HandleMomoIpnAsync(JObject payload);
        Task<BaseResponseModel<string>> QueryVnPayStatusAsync(QueryVnPayModel model);
        Task<BaseResponseModel<string>> QueryMoMoStatusAsync(QueryMoMoModel model);

    }
}
