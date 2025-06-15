using BeautySpa.Core.Base;
using BeautySpa.ModelViews.PayPalModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IPayPalService
    {
        Task<BaseResponseModel<CreatePayPalPaymentResponse>> CreatePaymentAsync(CreatePayPalPaymentRequest request);
        Task<BaseResponseModel<string>> ExecutePaymentAsync(ExecutePayPalPaymentRequest request);
        Task<BaseResponseModel<RefundPayPalResponse>> RefundAsync(RefundPayPalRequest request);
        Task<BaseResponseModel<string>> ConfirmPayPalAsync(string paymentId);
    }
}