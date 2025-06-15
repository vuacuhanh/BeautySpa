using BeautySpa.Core.Base;
using BeautySpa.ModelViews.MoMoModelViews;
using Microsoft.AspNetCore.Http;


namespace BeautySpa.Contract.Services.Interface
{
    public interface IMomoService
    {
        Task<BaseResponseModel<MomoCreatePaymentResponseModel>> CreatePaymentAsync(OrderInfoModel model);
        /*Task<BaseResponseModel<MomoRefundResponseModel>> RefundPaymentAsync(RefundRequest model);
        Task<BaseResponseModel<MomoTransactionStatusResponseModel>> QueryTransactionAsync(QueryMoMoModel model);*/
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
