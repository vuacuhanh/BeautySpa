using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.PayPalModelViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Payments;
using System.Net;
using Money = PayPalCheckoutSdk.Payments.Money;

namespace BeautySpa.Services.Service
{
    public class PayPalService : IPayPalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly ILogger<PayPalService> _logger;

        public PayPalService(IConfiguration config, ILogger<PayPalService> logger, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _logger = logger;
        }

        private PayPalEnvironment GetEnvironment()
        {
            var clientId = _config["PayPal:ClientId"];
            var clientSecret = _config["PayPal:ClientSecret"];
            return new SandboxEnvironment(clientId, clientSecret);
        }

        private PayPalHttpClient GetClient()
        {
            return new PayPalHttpClient(GetEnvironment());
        }

        public async Task<BaseResponseModel<CreatePayPalPaymentResponse>> CreatePaymentAsync(CreatePayPalPaymentRequest request)
        {
            var orderRequest = new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = request.Currency,
                            Value = request.Amount.ToString("F2")
                        },
                        Description = request.Description
                    }
                },
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = request.ReturnUrl,
                    CancelUrl = request.CancelUrl
                }
            };

            var createRequest = new OrdersCreateRequest();
            createRequest.Prefer("return=representation");
            createRequest.RequestBody(orderRequest);

            var client = GetClient();
            var response = await client.Execute(createRequest);

            if (response.StatusCode != HttpStatusCode.Created)
                return BaseResponseModel<CreatePayPalPaymentResponse>.Error(500, "Failed to create PayPal payment");

            var order = response.Result<Order>();
            var approvalLink = order.Links.FirstOrDefault(x => x.Rel == "approve")?.Href;

            return BaseResponseModel<CreatePayPalPaymentResponse>.Success(new CreatePayPalPaymentResponse
            {
                ApprovalUrl = approvalLink,
                PaymentId = order.Id
            });
        }

        public async Task<BaseResponseModel<string>> ExecutePaymentAsync(ExecutePayPalPaymentRequest request)
        {
            var captureRequest = new OrdersCaptureRequest(request.PaymentId);
            captureRequest.RequestBody(new OrderActionRequest());

            var client = GetClient();
            var response = await client.Execute(captureRequest);

            if (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.OK)
                return BaseResponseModel<string>.Error(500, "Failed to capture PayPal payment");

            return BaseResponseModel<string>.Success("Payment captured successfully");
        }

        public async Task<BaseResponseModel<RefundPayPalResponse>> RefundAsync(RefundPayPalRequest request)
        {
            var refundRequest = new CapturesRefundRequest(request.CaptureId);
            refundRequest.RequestBody(new RefundRequest
            {
                Amount = new Money
                {
                    CurrencyCode = request.Currency,
                    Value = request.Amount.ToString("F2")
                }
            });

            var client = GetClient();
            var response = await client.Execute(refundRequest);

            if (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.OK)
                return BaseResponseModel<RefundPayPalResponse>.Error(500, "Failed to refund PayPal payment");

            var refund = response.Result<PayPalCheckoutSdk.Payments.Refund>();

            return BaseResponseModel<RefundPayPalResponse>.Success(new RefundPayPalResponse
            {
                RefundId = refund.Id,
                Status = refund.Status
            });
        }

        public async Task<BaseResponseModel<string>> ConfirmPayPalAsync(string paymentId)
        {
            var captureRequest = new OrdersCaptureRequest(paymentId);
            captureRequest.RequestBody(new OrderActionRequest());

            var client = GetClient();
            var response = await client.Execute(captureRequest);

            if (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.OK)
                return BaseResponseModel<string>.Error(500, "Failed to capture PayPal payment");

            var payment = await _unitOfWork.GetRepository<Payment>()
                .Entities.FirstOrDefaultAsync(p =>
                    p.TransactionId == paymentId && p.PaymentMethod == "paypal");

            if (payment == null)
                return BaseResponseModel<string>.Error(404, "Không tìm thấy thanh toán tương ứng");

            payment.Status = "paid";
            payment.PaymentDate = CoreHelper.SystemTimeNow.UtcDateTime;

            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Payment captured and recorded successfully");
        }
    }
}
