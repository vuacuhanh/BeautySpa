using BeautySpa.Core.Base;
using BeautySpa.Core.Settings;
using BeautySpa.ModelViews.MoMoModelViews;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using BeautySpa.Services.Interface;

namespace BeautySpa.Services.Service
{
    public class MoMoService : IMomoService
    {
        private readonly HttpClient _httpClient;
        private readonly MomoSettings _momoSettings;

        public MoMoService(IHttpClientFactory httpClientFactory, IOptions<MomoSettings> momoOptions)
        {
            _httpClient = httpClientFactory.CreateClient();
            _momoSettings = momoOptions.Value;
        }

        public async Task<BaseResponseModel<CreatePaymentResponse>> CreatePaymentAsync(CreatePaymentRequest model)
        {
            if (string.IsNullOrEmpty(model.RequestId))
                model.RequestId = Guid.NewGuid().ToString();

            // Tạo rawData đúng thứ tự chuẩn của MoMo
            string rawData = $"partnerCode={_momoSettings.PartnerCode}&accessKey={_momoSettings.AccessKey}&requestId={model.RequestId}&amount={model.Amount}&orderId={model.OrderId}&orderInfo={model.OrderInfo}&returnUrl={_momoSettings.ReturnUrl}&notifyUrl={_momoSettings.NotifyUrl}&extraData={model.ExtraData}";
            string signature = ComputeHmacSha256(rawData, _momoSettings.SecretKey);

            var request = new
            {
                partnerCode = _momoSettings.PartnerCode,
                accessKey = _momoSettings.AccessKey,
                requestId = model.RequestId,
                amount = model.Amount,
                orderId = model.OrderId,
                orderInfo = model.OrderInfo,
                returnUrl = _momoSettings.ReturnUrl,
                notifyUrl = _momoSettings.NotifyUrl,
                requestType = _momoSettings.RequestType,
                extraData = model.ExtraData,
                signature = signature,
                lang = "vi"
            };

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_momoSettings.PaymentUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine("📦 MoMo RAW RESPONSE:");
            Console.WriteLine(responseContent);

            if (!response.IsSuccessStatusCode)
                throw new ErrorException(500, ErrorCode.InternalServerError, $"MoMo API lỗi HTTP {(int)response.StatusCode}");

            if (!IsJson(responseContent))
                throw new ErrorException(500, ErrorCode.InternalServerError, $"Phản hồi MoMo không phải JSON: {responseContent.Substring(0, Math.Min(300, responseContent.Length))}");

            var momoResponse = JsonConvert.DeserializeObject<CreatePaymentResponse>(responseContent)
                ?? throw new ErrorException(500, ErrorCode.InternalServerError, "Không thể phân tích JSON từ MoMo");

            if (momoResponse.ResultCode != 0)
                throw new ErrorException(400, ErrorCode.Failed, momoResponse.Message ?? "Thanh toán MoMo thất bại");

            return BaseResponseModel<CreatePaymentResponse>.Success(momoResponse);
        }

        public async Task<BaseResponseModel<RefundResponse>> RefundPaymentAsync(RefundRequest model)
        {
            if (string.IsNullOrEmpty(model.RequestId))
                model.RequestId = Guid.NewGuid().ToString();

            string rawData = $"partnerCode={_momoSettings.PartnerCode}" +
                $"&accessKey={_momoSettings.AccessKey}" +
                $"&requestId={model.RequestId}" +
                $"&amount={model.Amount}" +
                $"&orderId={model.OrderId}" +
                $"&transId={model.TransId}" +
                $"&requestType=refundMoMoWallet";
            string signature = ComputeHmacSha256(rawData, _momoSettings.SecretKey);

            var request = new
            {
                partnerCode = _momoSettings.PartnerCode,
                accessKey = _momoSettings.AccessKey,
                requestId = model.RequestId,
                amount = model.Amount,
                orderId = model.OrderId,
                transId = model.TransId,
                requestType = "refundMoMoWallet",
                signature = signature,
                lang = "vi"
            };

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_momoSettings.RefundUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new ErrorException(500, ErrorCode.InternalServerError, $"MoMo Refund HTTP lỗi {(int)response.StatusCode}");

            if (!IsJson(responseContent))
                throw new ErrorException(500, ErrorCode.InternalServerError, $"Phản hồi Refund không phải JSON: {responseContent.Substring(0, Math.Min(300, responseContent.Length))}");

            var momoResponse = JsonConvert.DeserializeObject<RefundResponse>(responseContent)
                ?? throw new ErrorException(500, ErrorCode.InternalServerError, "Không thể đọc JSON từ MoMo");

            if (int.TryParse(momoResponse.ResultCode, out var result) && result != 0)
                throw new ErrorException(400, ErrorCode.Failed, momoResponse.Message ?? "Lỗi hoàn tiền MoMo");

            return BaseResponseModel<RefundResponse>.Success(momoResponse);
        }

        public static string ComputeHmacSha256(string rawData, string secretKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private static bool IsJson(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            input = input.Trim();
            return (input.StartsWith("{") && input.EndsWith("}")) || (input.StartsWith("[") && input.EndsWith("]"));
        }
    }
}
