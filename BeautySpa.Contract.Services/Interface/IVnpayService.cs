using BeautySpa.Core.Base;
using BeautySpa.ModelViews.VnPayModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IVnpayService
    {
        Task<BaseResponseModel<CreateVnPayResponse>> CreatePaymentAsync(CreateVnPayRequest model);
        Task<BaseResponseModel<RefundVnPayResponse>> RefundPaymentAsync(RefundVnPayRequest model);
    }
}
