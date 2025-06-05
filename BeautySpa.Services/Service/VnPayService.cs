using System.Security.Cryptography;
using System.Text;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.ModelViews.VnPayModelViews;
using BeautySpa.Services.Validations.VnPayValidator;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BeautySpa.Services.Service
{
    public class VnpayService : IVnpayService
    {
        private readonly IConfiguration _config; private readonly IHttpContextAccessor _contextAccessor; private readonly IHttpClientFactory _httpClientFactory;

        public VnpayService(IConfiguration config, IHttpContextAccessor contextAccessor, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _contextAccessor = contextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        //public async Task<BaseResponseModel<CreateVnPayResponse>> CreatePaymentAsync(CreateVnPayRequest model)
        //{
        //    await new CreateVnPayValidator().ValidateAndThrowAsync(model);

        //    var vnp_Url = _config["VnPay:PaymentUrl"] ?? throw new ErrorException(500, ErrorCode.InternalServerError, "VNPAY PaymentUrl is missing");
        //    var vnp_TmnCode = _config["VnPay:TmnCode"] ?? throw new ErrorException(500, ErrorCode.InternalServerError, "VNPAY TmnCode is missing");
        //    var vnp_HashSecret = _config["VnPay:HashSecret"] ?? throw new ErrorException(500, ErrorCode.InternalServerError, "VNPAY HashSecret is missing");

        //    var vnp_ReturnUrl = model.ReturnUrl;
        //    var vnp_TxnRef = model.AppointmentId.ToString();
        //    var vnp_Amount = (long)(model.Amount * 100);
        //    var vnp_IpAddr = _contextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        //    var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");

        //    var inputData = new Dictionary<string, string?>
        //    {
        //        { "vnp_Version", "2.1.0" },
        //        { "vnp_Command", "pay" },
        //        { "vnp_TmnCode", vnp_TmnCode },
        //        { "vnp_Amount", vnp_Amount.ToString() },
        //        { "vnp_CurrCode", "VND" },
        //        { "vnp_TxnRef", vnp_TxnRef },
        //        { "vnp_OrderInfo", model.OrderInfo },
        //        { "vnp_OrderType", "other" },
        //        { "vnp_Locale", "vn" },
        //        { "vnp_ReturnUrl", vnp_ReturnUrl },
        //        { "vnp_IpAddr", vnp_IpAddr },
        //        { "vnp_CreateDate", vnp_CreateDate }
        //    };

        //    var sortedData = inputData
        //    .Where(x => x.Value != null)
        //    .OrderBy(x => x.Key)
        //    .ToDictionary(x => x.Key, x => x.Value!);

        //    string signRaw = string.Join("&", sortedData.Select(x => $"{x.Key}={x.Value}"));
        //    string secureHash = ComputeSha256(signRaw + vnp_HashSecret);

        //    inputData.Add("vnp_SecureHashType", "SHA256");
        //    inputData.Add("vnp_SecureHash", secureHash);

        //    var queryString = string.Join('&', inputData
        //        .Where(x => x.Value != null)
        //        .Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value!)}"));
        //    var payUrl = $"{vnp_Url}?{queryString}";

        //    var response = new CreateVnPayResponse
        //    {
        //        PayUrl = payUrl,
        //        TransactionId = vnp_TxnRef
        //    };

        //    return BaseResponseModel<CreateVnPayResponse>.Success(response);
        //}

        public async Task<BaseResponseModel<CreateVnPayResponse>> CreatePaymentAsync(CreateVnPayRequest model)
        {
            await new CreateVnPayValidator().ValidateAndThrowAsync(model);

            var vnp_Url = _config["VnPay:PaymentUrl"] ?? throw new ErrorException(500, ErrorCode.InternalServerError, "VNPAY PaymentUrl is missing");
            var vnp_TmnCode = _config["VnPay:TmnCode"] ?? throw new ErrorException(500, ErrorCode.InternalServerError, "VNPAY TmnCode is missing");
            var vnp_HashSecret = _config["VnPay:HashSecret"] ?? throw new ErrorException(500, ErrorCode.InternalServerError, "VNPAY HashSecret is missing");

            var vnp_ReturnUrl = model.ReturnUrl ?? throw new ErrorException(400, ErrorCode.Unknown, "Missing ReturnUrl");
            var vnp_TxnRef = model.AppointmentId.ToString();
            var vnp_Amount = (long)(model.Amount * 100);
            var vnp_IpAddr = _contextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");

            var inputData = new Dictionary<string, string?>
    {
        { "vnp_Version", "2.1.0" },
        { "vnp_Command", "pay" },
        { "vnp_TmnCode", vnp_TmnCode },
        { "vnp_Amount", vnp_Amount.ToString() },
        { "vnp_CurrCode", "VND" },
        { "vnp_TxnRef", vnp_TxnRef },
        { "vnp_OrderInfo", model.OrderInfo },
        { "vnp_OrderType", "other" },
        { "vnp_Locale", "vn" },
        { "vnp_ReturnUrl", vnp_ReturnUrl },
        { "vnp_IpAddr", vnp_IpAddr },
        { "vnp_CreateDate", vnp_CreateDate }
    };

            // ✅ Tạo raw data để ký (KHÔNG escape)
            var sortedData = inputData
                .Where(x => x.Value != null)
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Value!);

            string signRaw = string.Join("&", sortedData.Select(x => $"{x.Key}={x.Value}"));
            string secureHash = ComputeSha256(signRaw + vnp_HashSecret);

            // ✅ Thêm chữ ký vào dữ liệu
            inputData.Add("vnp_SecureHashType", "SHA256");
            inputData.Add("vnp_SecureHash", secureHash);

            // ✅ Tạo query string có escape
            var queryString = string.Join("&", inputData
                .Where(x => x.Value != null)
                .Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value!)}"));

            var payUrl = $"{vnp_Url}?{queryString}";

            var response = new CreateVnPayResponse
            {
                PayUrl = payUrl,
                TransactionId = vnp_TxnRef
            };

            return BaseResponseModel<CreateVnPayResponse>.Success(response);
        }


        public async Task<BaseResponseModel<RefundVnPayResponse>> RefundPaymentAsync(RefundVnPayRequest model)
        {
            await new RefundVnPayValidator().ValidateAndThrowAsync(model);

            var vnp_Url = _config["VnPay:RefundUrl"] ?? throw new ErrorException(500, ErrorCode.InternalServerError, "VNPAY RefundUrl is missing");
            var vnp_TmnCode = _config["VnPay:TmnCode"] ?? throw new ErrorException(500, ErrorCode.InternalServerError, "VNPAY TmnCode is missing");
            var vnp_HashSecret = _config["VnPay:HashSecret"] ?? throw new ErrorException(500, ErrorCode.InternalServerError, "VNPAY HashSecret is missing");
            var vnp_IpAddr = _contextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "127.0.0.1";

            var inputData = new Dictionary<string, string?>
            {
                { "vnp_RequestId", Guid.NewGuid().ToString() },
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "refund" },
                { "vnp_TmnCode", vnp_TmnCode },
                { "vnp_TransactionType", "02" },
                { "vnp_TxnRef", model.TransactionId },
                { "vnp_Amount", ((long)(model.Amount * 100)).ToString() },
                { "vnp_OrderInfo", model.Reason },
                { "vnp_TransactionDate", model.TransactionDate?.ToString("yyyyMMddHHmmss") ?? throw new ErrorException(400, ErrorCode.InvalidInput, "TransactionDate is required") },
                { "vnp_CreateBy", CurrentUserId },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_IpAddr", vnp_IpAddr }
            };

            string signData = string.Join('&', inputData
                .Where(x => x.Value != null)
                .OrderBy(x => x.Key)
                .Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value!)}"));
            string secureHash = ComputeSha256(signData + vnp_HashSecret);

            inputData.Add("vnp_SecureHashType", "SHA256");
            inputData.Add("vnp_SecureHash", secureHash);

            var client = _httpClientFactory.CreateClient();
            var content = new FormUrlEncodedContent(inputData.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value!));
            var response = await client.PostAsync(vnp_Url, content);

            if (!response.IsSuccessStatusCode)
                throw new ErrorException(500, ErrorCode.InternalServerError, $"VNPAY API returned {response.StatusCode}");

            var result = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(result)
                ?? throw new ErrorException(500, ErrorCode.InternalServerError, "VNPAY refund response is null");

            var responseModel = new RefundVnPayResponse
            {
                ResponseCode = json.vnp_ResponseCode ?? "99",
                Message = json.vnp_Message ?? "Refund failed",
                TransactionId = model.TransactionId
            };

            if (responseModel.ResponseCode != 0)
                throw new ErrorException(400, ErrorCode.Failed, responseModel.Message);

            return BaseResponseModel<RefundVnPayResponse>.Success(responseModel);
        }

        public static string ComputeSha256(string rawData)
        {
            using SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

}