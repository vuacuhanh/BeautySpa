using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.VnPayModelViews;
using BeautySpa.Services.Validations.VnPayValidator;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Http;

namespace BeautySpa.Services.Service
{
    public class VnpayService : IVnpayService
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _contextAccessor;

        public VnpayService(IConfiguration config, IHttpContextAccessor contextAccessor)
        {
            _config = config;
            _contextAccessor = contextAccessor;
        }

        public async Task<BaseResponseModel<CreateVnPayResponse>> CreatePaymentAsync(CreateVnPayRequest model)
        {
            await new CreateVnPayValidator().ValidateAndThrowAsync(model);

            var vnp_Url = _config["VnPay:PaymentUrl"] ?? "";
            var vnp_ReturnUrl = model.ReturnUrl;
            var vnp_TmnCode = _config["VnPay:TmnCode"] ?? "";
            var vnp_HashSecret = _config["VnPay:HashSecret"] ?? "";

            string vnp_TxnRef = $"appointment_{model.AppointmentId}";
            string vnp_OrderInfo = model.OrderInfo;
            string vnp_Amount = ((long)(model.Amount * 100)).ToString(); // multiply x100 theo spec của VNPAY
            string vnp_Locale = model.Locale ?? "vn";
            string vnp_BankCode = model.BankCode;
            string vnp_IpAddr = _contextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "127.0.0.1";

            var inputData = new Dictionary<string, string?>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", vnp_TmnCode },
                { "vnp_Amount", vnp_Amount },
                { "vnp_CurrCode", "VND" },
                { "vnp_TxnRef", vnp_TxnRef },
                { "vnp_OrderInfo", vnp_OrderInfo },
                { "vnp_OrderType", "other" },
                { "vnp_Locale", vnp_Locale },
                { "vnp_ReturnUrl", vnp_ReturnUrl },
                { "vnp_IpAddr", vnp_IpAddr },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") }
            };

            if (!string.IsNullOrEmpty(vnp_BankCode))
                inputData.Add("vnp_BankCode", vnp_BankCode);

            string signData = string.Join('&', inputData.OrderBy(x => x.Key).Select(x => $"{x.Key}={x.Value}"));
            string secureHash = ComputeSha256(signData + vnp_HashSecret);

            inputData.Add("vnp_SecureHashType", "SHA256");
            inputData.Add("vnp_SecureHash", secureHash);

            string paymentUrl = QueryHelpers.AddQueryString(vnp_Url, inputData);


            var result = new CreateVnPayResponse
            {
                PayUrl = paymentUrl,
                TransactionId = vnp_TxnRef,
                Message = "VNPAY URL created"
            };

            return BaseResponseModel<CreateVnPayResponse>.Success(result);
        }

        public async Task<BaseResponseModel<RefundVnPayResponse>> RefundPaymentAsync(RefundVnPayRequest model)
        {
            await new RefundVnPayValidator().ValidateAndThrowAsync(model);

            // TODO: Nếu có API VNPAY thực tế thì gọi ở đây. Tạm giả lập hoàn tiền thành công.
            var response = new RefundVnPayResponse
            {
                ResponseCode = 0,
                Message = "Hoàn tiền thành công",
                TransactionId = model.TransactionId
            };

            return BaseResponseModel<RefundVnPayResponse>.Success(response);
        }

        private static string ComputeSha256(string data)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = sha256.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
