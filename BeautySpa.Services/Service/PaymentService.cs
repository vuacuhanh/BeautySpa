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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
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
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContext,
            IMomoService momoService,
            IVnpayService vnpayService,
            IConfiguration config,
            IHttpClientFactory httpClientFactory)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContext = httpContext;
            _momoService = momoService;
            _vnpayService = vnpayService;
            _config = config;
            _httpClientFactory = httpClientFactory;
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

            var userId = Authentication.GetUserIdFromHttpContextAccessor(_httpContext);

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

            if (model.PaymentMethod?.ToLower() == "momo")
            {
                var request = new CreatePaymentRequest
                {
                    OrderId = appointment.Id.ToString(),
                    Amount = (long)model.Amount,
                    OrderInfo = $"Thanh toán cọc lịch hẹn #{appointment.Id}"
                };
                var momoResp = await _momoService.CreatePaymentAsync(request);
                if (momoResp.Data == null || momoResp.Data.ResultCode != 0)
                    throw new ErrorException(400, ErrorCode.Failed, momoResp.Data?.Message ?? "Lỗi khi tạo thanh toán MoMo");

                payment.TransactionId = momoResp.Data.RequestId;
                payUrl = momoResp.Data.PayUrl;
            }
            else if (model.PaymentMethod?.ToLower() == "vnpay")
            {
                var request = new CreateVnPayRequest
                {
                    AppointmentId = appointment.Id,
                    Amount = model.Amount,
                    OrderInfo = $"Thanh toán cọc lịch hẹn #{appointment.Id}",
                    ReturnUrl = _config["VnPay:ReturnUrl"] ?? throw new ErrorException(500, ErrorCode.InternalServerError, "Missing ReturnUrl")
                };
                var vnpayResp = await _vnpayService.CreatePaymentAsync(request);
                if (vnpayResp.Data == null || string.IsNullOrEmpty(vnpayResp.Data.PayUrl))
                    throw new ErrorException(400, ErrorCode.Failed, vnpayResp.Data?.Message ?? "Lỗi khi tạo thanh toán VNPAY");

                payment.TransactionId = vnpayResp.Data.TransactionId;
                payUrl = vnpayResp.Data.PayUrl;
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
                PaymentMethod = model.PaymentMethod ?? string.Empty,
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
                    TransId = long.TryParse(payment.TransactionId, out var tid) ? tid : throw new ErrorException(400, ErrorCode.InvalidInput, "Invalid MoMo TransactionId"),
                    Description = model.Reason
                };
                var result = await _momoService.RefundPaymentAsync(request);
                if (result.Data?.ResultCode != "0")
                    throw new ErrorException(400, ErrorCode.Failed, result.Data?.Message ?? "Refund failed");
            }
            else if (payment.PaymentMethod.ToLower() == "vnpay")
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

        public async Task<BasePaginatedList<GETPaymentModelView>> GetPaymentHistoryAsync(int pageIndex, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Payment>()
                .Entities.AsNoTracking()
                .Where(p => p.CreatedBy == CurrentUserId && p.DeletedTime == null)
                .OrderByDescending(p => p.CreatedTime);

            var paginatedList = await _unitOfWork.GetRepository<Payment>().GetPagging(query, pageIndex, pageSize);
            var result = new BasePaginatedList<GETPaymentModelView>(
                paginatedList.Items.Select(p => _mapper.Map<GETPaymentModelView>(p)).ToList(),
                paginatedList.TotalItems, paginatedList.CurrentPage, paginatedList.PageSize);

            return result;
        }

        public async Task<BaseResponseModel<string>> HandleVnpayIpnAsync(Dictionary<string, string> query)
        {
            string? receivedSecureHash = query.GetValueOrDefault("vnp_SecureHash");
            string? txnRef = query.GetValueOrDefault("vnp_TxnRef");
            string? responseCode = query.GetValueOrDefault("vnp_ResponseCode");

            if (string.IsNullOrEmpty(receivedSecureHash) || string.IsNullOrEmpty(txnRef))
                throw new ErrorException(400, ErrorCode.InvalidInput, "Missing required parameters");

            var hashSecret = _config["VnPay:HashSecret"] ?? throw new ErrorException(500, ErrorCode.InternalServerError, "VNPAY HashSecret missing");

            var signData = string.Join("&", query
                .Where(x => x.Key != "vnp_SecureHash" && x.Key != "vnp_SecureHashType")
                .OrderBy(x => x.Key)
                .Select(x => $"{x.Key}={x.Value}"));

            var computedHash = VnpayService.ComputeSha256(signData + hashSecret);
            if (!string.Equals(computedHash, receivedSecureHash, StringComparison.OrdinalIgnoreCase))
                throw new ErrorException(400, ErrorCode.InvalidInput, "Invalid VNPAY signature");

            var payment = await _unitOfWork.GetRepository<Payment>()
                .Entities.FirstOrDefaultAsync(p => p.TransactionId == txnRef && p.DeletedTime == null)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Payment not found");

            if (responseCode == "00")
            {
                payment.Status = "completed";
                payment.PaymentDate = CoreHelper.SystemTimeNow.UtcDateTime;
                payment.LastUpdatedTime = CoreHelper.SystemTimeNow;
                await _unitOfWork.SaveAsync();
                return BaseResponseModel<string>.Success("VNPAY payment confirmed");
            }

            throw new ErrorException(400, ErrorCode.Failed, "VNPAY transaction failed");
        }

        public async Task<BaseResponseModel<string>> HandleMomoIpnAsync(JObject payload)
        {
            string? orderId = payload["orderId"]?.ToString();
            string? resultCode = payload["resultCode"]?.ToString();
            string? transIdStr = payload["transId"]?.ToString();

            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(resultCode))
                throw new ErrorException(400, ErrorCode.InvalidInput, "Invalid MoMo payload");

            var appointmentId = ExtractAppointmentId(orderId);
            var payment = await _unitOfWork.GetRepository<Payment>()
                .Entities.FirstOrDefaultAsync(p => p.AppointmentId == appointmentId && p.DeletedTime == null)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Payment not found");

            if (resultCode == "0")
            {
                payment.Status = "completed";
                payment.TransactionId = transIdStr;
                payment.PaymentDate = CoreHelper.SystemTimeNow.UtcDateTime;
                payment.LastUpdatedTime = CoreHelper.SystemTimeNow;
                await _unitOfWork.SaveAsync();
                return BaseResponseModel<string>.Success("MoMo payment confirmed");
            }

            throw new ErrorException(400, ErrorCode.Failed, "MoMo transaction failed");
        }

        private Guid ExtractAppointmentId(string orderId)
        {
            var parts = orderId.Split('_');
            if (parts.Length == 2 && Guid.TryParse(parts[1], out var id)) return id;
            throw new ErrorException(400, ErrorCode.InvalidInput, "Invalid appointment ID format in OrderId");
        }

        public async Task<BaseResponseModel<string>> QueryMoMoStatusAsync(QueryMoMoModel model)
        {
            string endpoint = _config["MoMo:QueryUrl"] ?? "https://test-payment.momo.vn/v2/gateway/api/query";
            string partnerCode = _config["MoMo:PartnerCode"] ?? throw new ErrorException(500, ErrorCode.InternalServerError, "MoMo PartnerCode missing");
            string accessKey = _config["MoMo:AccessKey"] ?? throw new ErrorException(500, ErrorCode.InternalServerError, "MoMo AccessKey missing");
            string secretKey = _config["MoMo:SecretKey"] ?? throw new ErrorException(500, ErrorCode.InternalServerError, "MoMo SecretKey missing");

            string rawHash = $"accessKey={accessKey}&orderId={model.OrderId}&partnerCode={partnerCode}&requestId={model.RequestId}";
            string signature = MoMoService.ComputeHmacSha256(rawHash, secretKey);

            var body = new { partnerCode, requestId = model.RequestId, orderId = model.OrderId, lang = "vi", signature };
            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);

            if (!response.IsSuccessStatusCode)
                throw new ErrorException(500, ErrorCode.InternalServerError, $"MoMo Query API failed with status {response.StatusCode}");

            var json = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            string resultCode = json!.resultCode ?? "unknown";
            string message = json.message ?? "No message";

            return BaseResponseModel<string>.Success($"MoMo resultCode: {resultCode} - {message}");
        }

        public async Task<BaseResponseModel<string>> QueryVnPayStatusAsync(QueryVnPayModel model)
        {
            string endpoint = _config["VnPay:RefundUrl"] ?? "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction";
            string tmnCode = _config["VnPay:TmnCode"] ?? throw new ErrorException(500, ErrorCode.InternalServerError, "VNPAY TmnCode missing");
            string hashSecret = _config["VnPay:HashSecret"] ?? throw new ErrorException(500, ErrorCode.InternalServerError, "VNPAY HashSecret missing");

            var requestId = Guid.NewGuid().ToString("N");
            var transDate = model.TransactionDate.ToString("yyyyMMddHHmmss");

            var inputData = new Dictionary<string, string>
            {
                { "vnp_RequestId", requestId },
                { "vnp_Command", "querydr" },
                { "vnp_TmnCode", tmnCode },
                { "vnp_TxnRef", model.TransactionId },
                { "vnp_OrderInfo", "Query transaction status" },
                { "vnp_TransactionDate", transDate },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_Version", "2.1.0" },
                { "vnp_IpAddr", "127.0.0.1" }
            };

            string signData = string.Join("&", inputData.OrderBy(x => x.Key).Select(x => $"{x.Key}={x.Value}"));
            string secureHash = VnpayService.ComputeSha256(signData + hashSecret);
            inputData.Add("vnp_SecureHashType", "SHA256");
            inputData.Add("vnp_SecureHash", secureHash);

            var client = _httpClientFactory.CreateClient();
            var content = new FormUrlEncodedContent(inputData);
            var response = await client.PostAsync(endpoint, content);

            if (!response.IsSuccessStatusCode)
                throw new ErrorException(500, ErrorCode.InternalServerError, $"VNPAY query API failed with {response.StatusCode}");

            var json = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            string respCode = json!.vnp_ResponseCode ?? "unknown";
            string transStatus = json.vnp_TransactionStatus ?? "unknown";
            string message = json.vnp_Message ?? "no message";

            return BaseResponseModel<string>.Success($"VNPAY ResponseCode: {respCode}, Status: {transStatus}, Message: {message}");
        }
    }
}