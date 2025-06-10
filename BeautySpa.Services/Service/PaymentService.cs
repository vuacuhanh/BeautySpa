using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.PaymentModelViews;
using BeautySpa.ModelViews.MoMoModelViews;
using BeautySpa.ModelViews.VnPayModelViews;
using BeautySpa.ModelViews.PayPalModelViews;
using BeautySpa.Services.Validations.PaymentValidator;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using BeautySpa.Services.Interface;

namespace BeautySpa.Services.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMomoService _momoService;
        private readonly IVnpayService _vnpayService;
        private readonly IPayPalService _paypalService;
        private readonly IConfiguration _config;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContext,
            IMomoService momoService,
            IVnpayService vnpayService,
            IPayPalService paypalService,
            IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContext = httpContext;
            _momoService = momoService;
            _vnpayService = vnpayService;
            _paypalService = paypalService;
            _config = config;
        }

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_httpContext);

        public async Task<BaseResponseModel<PaymentResponse>> CreateDepositAsync(POSTPaymentModelView model)
        {
            await new POSTPaymentValidator().ValidateAndThrowAsync(model);

            var appointment = await _unitOfWork.GetRepository<Appointment>()
                .Entities.Include(x => x.Payment)
                .FirstOrDefaultAsync(x => x.Id == model.AppointmentId && x.DeletedTime == null)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Appointment not found");

            if (appointment.Payment != null && appointment.Payment.Status != "refunded")
                throw new ErrorException(400, ErrorCode.Duplicate, "Appointment has been paid deposit");

            var userId = CurrentUserId;
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                AppointmentId = appointment.Id,
                Amount = model.Amount,
                PaymentMethod = model.PaymentMethod,
                CreatedBy = userId,
                CreatedTime = CoreHelper.SystemTimeNow,
                Status = "pending"
            };

            string? payUrl = null;
            var method = model.PaymentMethod?.Trim().ToLowerInvariant();

            if (method == "paypal")
            {
                var request = new CreatePayPalPaymentRequest
                {
                    Amount = model.Amount,
                    Description = $"Thanh toán cọc lịch hẹn #{appointment.Id}",
                    ReturnUrl = _config["Frontend:PaypalReturnUrl"],
                    CancelUrl = _config["Frontend:PaypalCancelUrl"]
                };

                var paypalResp = await _paypalService.CreatePaymentAsync(request);
                if (paypalResp.Data == null || string.IsNullOrEmpty(paypalResp.Data.PaymentId))
                    throw new ErrorException(400, ErrorCode.Failed, "Lỗi khi tạo thanh toán PayPal");

                payment.TransactionId = paypalResp.Data.PaymentId;
                payUrl = paypalResp.Data.ApprovalUrl;
            }

            else if (method == "vnpay")
            {
                var request = new CreateVnPayRequest
                {
                    AppointmentId = appointment.Id,
                    Amount = model.Amount,
                    OrderInfo = $"{appointment.Id}",
                    ReturnUrl = _config["VnPay:ReturnUrl"] ?? throw new ErrorException(500, ErrorCode.InternalServerError, "Missing ReturnUrl")
                };
                var vnpayResp = await _vnpayService.CreatePaymentAsync(request);
                if (vnpayResp.Data == null || vnpayResp.Data.TransactionId != "00")
                    throw new ErrorException(400, ErrorCode.Failed, vnpayResp.Data?.Message ?? "Lỗi khi tạo thanh toán VNPAY");

                payment.TransactionId = vnpayResp.Data.TransactionId;
                payUrl = vnpayResp.Data.PayUrl;
            }
            else if (method == "momo")
            {
                var request = new CreatePaymentRequest
                {
                    OrderId = appointment.Id.ToString(),
                    Amount = model.Amount,
                    OrderInfo = $"{appointment.Id}"
                };
                var momoResp = await _momoService.CreatePaymentAsync(request);
                if (momoResp.Data == null || momoResp.Data.ResultCode != 0)
                    throw new ErrorException(400, ErrorCode.Failed, momoResp.Data?.Message ?? "Lỗi khi tạo thanh toán MoMo");

                payment.TransactionId = momoResp.Data.RequestId;
                payUrl = momoResp.Data.PayUrl;
            }

            else
            {
                throw new ErrorException(400, ErrorCode.InvalidInput, "Phương thức thanh toán không hợp lệ");
            }

            await _unitOfWork.GetRepository<Payment>().InsertAsync(payment);
            await _unitOfWork.SaveAsync();

            var response = new PaymentResponse
            {
                AppointmentId = appointment.Id,
                Amount = model.Amount,
                PaymentMethod = method,
                PayUrl = payUrl ?? string.Empty
            };

            return BaseResponseModel<PaymentResponse>.Success(response);
        }

        public async Task<BaseResponseModel<string>> RefundDepositAsync(RefundPaymentModelView model)
        {
            await new RefundPaymentValidator().ValidateAndThrowAsync(model);
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            var payment = await _unitOfWork.GetRepository<Payment>()
                .Entities.FirstOrDefaultAsync(p => p.AppointmentId == model.AppointmentId && p.DeletedTime == null)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "No transaction found to refund");

            if (payment.Status == "refunded")
                throw new ErrorException(400, ErrorCode.Duplicate, "Transaction has been refunded");

            int refundAmount = (int)payment.Amount;
            int fee = 0;

            if (model.KeepPlatformFee)
            {
                fee = (int)Math.Round(payment.Amount * 0.1m, MidpointRounding.AwayFromZero);
                refundAmount -= fee;
            }

            var method = payment.PaymentMethod.ToLower();
            if (method == "paypal")
            {
                var request = new RefundPayPalRequest
                {
                    CaptureId = payment.TransactionId!,
                    Amount = refundAmount,
                    Currency = "USD"
                };
                var result = await _paypalService.RefundAsync(request);
                if (result.Data == null || result.Data.Status?.ToLower() != "completed")
                    throw new ErrorException(400, ErrorCode.Failed, "Refund failed");
            }
                
            else if (method == "vnpay")
            {
                var request = new RefundVnPayRequest
                {
                    TransactionId = payment.TransactionId!,
                    Amount = refundAmount,
                    Reason = model.Reason,
                    TransactionDate = payment.PaymentDate
                };
                var result = await _vnpayService.RefundPaymentAsync(request);
                if (result.Data?.ResponseCode != 0)
                    throw new ErrorException(400, ErrorCode.Failed, result.Data?.Message ?? "Refund failed");
            }
            else if (method == "momo")
            {
                var request = new RefundRequest
                {
                    OrderId = $"appointment_{model.AppointmentId}",
                    RequestId = Guid.NewGuid().ToString(),
                    Amount = refundAmount,
                    TransId = payment.TransactionId!
                };
                var result = await _momoService.RefundPaymentAsync(request);
                if (result.Data?.ResultCode != "0")
                    throw new ErrorException(400, ErrorCode.Failed, result.Data?.Message ?? "Refund failed");
            }

            payment.RefundAmount = refundAmount;
            payment.PlatformFee = fee;
            payment.Status = "refunded";
            payment.LastUpdatedBy = CurrentUserId;
            payment.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Payment>().UpdateAsync(payment);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            return BaseResponseModel<string>.Success("Refund successful");
        }

        public async Task<BaseResponseModel<GETPaymentModelView>> GetByAppointmentIdAsync(Guid appointmentId)
        {
            var payment = await _unitOfWork.GetRepository<Payment>()
                .Entities.AsNoTracking()
                .FirstOrDefaultAsync(p => p.AppointmentId == appointmentId && p.DeletedTime == null)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Payment not found");

            var result = _mapper.Map<GETPaymentModelView>(payment);
            return BaseResponseModel<GETPaymentModelView>.Success(result);
        }

        public async Task<BaseResponseModel<string>> ConfirmPayPalAsync(string paymentId)
        {
            var result = await _paypalService.ExecutePaymentAsync(new ExecutePayPalPaymentRequest
            {
                PaymentId = paymentId
            });

            if (!result.IsSuccess)
                throw new ErrorException(400, ErrorCode.Failed, result.Message ?? "Payment confirmation failed");

            var payment = await _unitOfWork.GetRepository<Payment>()
                .Entities.FirstOrDefaultAsync(p => p.TransactionId == paymentId && p.Status == "pending");

            if (payment == null)
                throw new ErrorException(404, ErrorCode.NotFound, "Payment not found");

            payment.Status = "completed";
            payment.LastUpdatedTime = CoreHelper.SystemTimeNow;
            payment.LastUpdatedBy = Authentication.GetUserIdFromHttpContextAccessor(_httpContext);

            await _unitOfWork.GetRepository<Payment>().UpdateAsync(payment);

            var appointment = await _unitOfWork.GetRepository<Appointment>()
                .Entities.FirstOrDefaultAsync(x => x.Id == payment.AppointmentId && x.DeletedTime == null);

            if (appointment != null)
            {
                appointment.BookingStatus = "pending"; // vẫn pending vì spa chưa xác nhận
                appointment.LastUpdatedTime = CoreHelper.SystemTimeNow;
                appointment.LastUpdatedBy = Authentication.GetUserIdFromHttpContextAccessor(_httpContext);
                await _unitOfWork.GetRepository<Appointment>().UpdateAsync(appointment);
            }

            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Payment confirmed successfully");
        }

    }
}
