using BeautySpa.Core.Base;
using BeautySpa.ModelViews.MoMoModelViews;

namespace BeautySpa.Services.Interface
{
    public interface IMomoService
    {
        Task<BaseResponseModel<CreatePaymentResponse>> CreatePaymentAsync(CreatePaymentRequest model);
        Task<BaseResponseModel<RefundResponse>> RefundPaymentAsync(RefundRequest model);
    }
}
