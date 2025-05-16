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
using BeautySpa.Services.Validations.PaymentValidator;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace BeautySpa.Services.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMomoService _momoService;
        private readonly IVnpayService _vnpayService;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContext,
            IMomoService momoService,
            IVnpayService vnpayService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContext = httpContext;
            _momoService = momoService;
            _vnpayService = vnpayService;
        }

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_httpContext);

        public async Task<BaseResponseModel<string>> CreateDepositAsync(POSTPaymentModelView model)
        {
            await new POSTPaymentValidator().ValidateAndThrowAsync(model);

            var appointment = await _unitOfWork.GetRepository<Appointment>()
                .Entities.Include(x => x.Payment)
                .FirstOrDefaultAsync(x => x.Id == model.AppointmentId && x.DeletedTime == null)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Appointment does not exist");

            if (appointment.Payment != null)
                throw new ErrorException(400, ErrorCode.Duplicate, "Appointment has been paid deposit");

            var payment = _mapper.Map<Payment>(model);
            payment.Id = Guid.NewGuid();
            payment.AppointmentId = appointment.Id;
            payment.CreatedBy = CurrentUserId;
            payment.CreatedTime = CoreHelper.SystemTimeNow;
            payment.Status = "deposit_paid";
            payment.TransactionType = "Deposit";

            BaseResponseModel<CreatePaymentResponse>? momoResp = null;
            BaseResponseModel<CreateVnPayResponse>? vnpayResp = null;

            if (model.PaymentMethod.ToLower() == "momo")
            {
                var request = new CreatePaymentRequest
                {
                    Amount = (long)model.Amount,
                    OrderId = $"appointment_{appointment.Id}",
                    OrderInfo = $"Deposit payment for appointment #{appointment.Id}"
                };
                momoResp = await _momoService.CreatePaymentAsync(request);
                payment.TransactionId = momoResp.Data?.RequestId;
            }
            else if (model.PaymentMethod.ToLower() == "vnpay")
            {
                var request = new CreateVnPayRequest
                {
                    AppointmentId = appointment.Id,
                    Amount = model.Amount,
                    OrderInfo = $"Deposit payment for appointment #{appointment.Id}",
                    ReturnUrl = "https://spa-client.com/payment-callback"
                };
                vnpayResp = await _vnpayService.CreatePaymentAsync(request);
                payment.TransactionId = vnpayResp.Data?.TransactionId;
            }
            else
                throw new ErrorException(400, ErrorCode.InvalidInput, "Invalid payment method   ");

            await _unitOfWork.GetRepository<Payment>().InsertAsync(payment);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success(model.PaymentMethod.ToLower() == "momo" ? momoResp?.Data?.PayUrl : vnpayResp?.Data?.PayUrl);
        }

        public async Task<BaseResponseModel<string>> RefundDepositAsync(RefundPaymentModelView model)
        {
            await new RefundPaymentValidator().ValidateAndThrowAsync(model);

            var payment = await _unitOfWork.GetRepository<Payment>()
                .Entities.FirstOrDefaultAsync(p => p.AppointmentId == model.AppointmentId && p.DeletedTime == null)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "No transaction found to refund");

            if (payment.Status == "refunded")
                throw new ErrorException(400, ErrorCode.Duplicate, "Transaction has been refunded");

            decimal refundAmount = payment.Amount;
            decimal fee = 0;

            if (model.KeepPlatformFee)
            {
                fee = Math.Round(payment.Amount * 0.1m, 0);
                refundAmount -= fee;
            }

            if (payment.PaymentMethod.ToLower() == "momo")
            {
                var request = new RefundRequest
                {
                    OrderId = $"appointment_{model.AppointmentId}",
                    RequestId = Guid.NewGuid().ToString(),
                    Amount = (long)refundAmount,
                    TransId = long.TryParse(payment.TransactionId, out var tid) ? tid : 0,
                    Description = model.Reason
                };

                var result = await _momoService.RefundPaymentAsync(request);
                if (result.Data?.ResultCode != "0")
                    throw new ErrorException(400, ErrorCode.Failed, result.Data?.Message ?? "Refund failed");
            }
            else if (payment.PaymentMethod.ToLower() == "vnpay")
            {
                var request = new RefundVnPayRequest(); // TODO: Bổ sung dữ liệu đầy đủ khi model không rỗng
                var result = await _vnpayService.RefundPaymentAsync(request);
                if (result.Data?.ResponseCode != 0)
                    throw new ErrorException(400, ErrorCode.Failed, result.Data?.Message ?? "Refund failed");
            }

            payment.RefundAmount = refundAmount;
            payment.PlatformFee = fee;
            payment.Status = "refunded";
            payment.LastUpdatedBy = CurrentUserId;
            payment.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Payment>().UpdateAsync(payment);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Refund successful");
        }

        public async Task<BaseResponseModel<GETPaymentModelView>> GetByAppointmentIdAsync(Guid appointmentId)
        {
            var payment = await _unitOfWork.GetRepository<Payment>()
                .Entities.AsNoTracking()
                .FirstOrDefaultAsync(p => p.AppointmentId == appointmentId && p.DeletedTime == null);

            if (payment == null)
                throw new ErrorException(404, ErrorCode.NotFound, "Payment not found");

            var result = _mapper.Map<GETPaymentModelView>(payment);
            return BaseResponseModel<GETPaymentModelView>.Success(result);
        }
    }
}
