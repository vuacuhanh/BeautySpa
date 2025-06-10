using BeautySpa.Core.Base;
using BeautySpa.ModelViews.MoMoModelViews;
using BeautySpa.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace BeautySpa.Services.Service
{
    public class MoMoService : IMomoService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MoMoService> _logger;

        public MoMoService(IConfiguration config, IHttpClientFactory httpClientFactory, ILogger<MoMoService> logger)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<BaseResponseModel<CreatePaymentResponse>> CreatePaymentAsync(CreatePaymentRequest model)
        {
            string endpoint = _config["MoMo:PaymentUrl"]!;
            string partnerCode = _config["MoMo:PartnerCode"]!;
            string accessKey = _config["MoMo:AccessKey"]!;
            string secretKey = _config["MoMo:SecretKey"]!;
            string redirectUrl = _config["MoMo:ReturnUrl"]!;
            string ipnUrl = _config["MoMo:NotifyUrl"]!;
            string requestType = _config["MoMo:RequestType"]!;

            string requestId = Guid.NewGuid().ToString();
            string orderId = model.OrderId;
            int amount = model.Amount; // Đảm bảo int

            string orderInfo = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(model.OrderInfo));
            string extraData = model.ExtraData ?? "";

            // Raw signature
            string rawHash =
                $"accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={ipnUrl}" +
                $"&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}" +
                $"&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";

            string signature = HmacSHA256(rawHash, secretKey);

            _logger.LogInformation("🔍 [MoMo] RawHash = {rawHash}", rawHash);
            _logger.LogInformation("🔐 [MoMo] Signature = {signature}", signature);

            var payload = new
            {
                partnerCode,
                accessKey,
                requestId,
                amount,
                orderId,
                orderInfo,
                redirectUrl,
                ipnUrl,
                extraData,
                requestType,
                signature,
                lang = "vi"
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);
            _logger.LogInformation("📤 [MoMo] Payload = {json}", jsonPayload);

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("📥 [MoMo] Raw Response:\n{resp}", responseContent);

            if (!response.IsSuccessStatusCode)
            {
                return BaseResponseModel<CreatePaymentResponse>.Error((int)response.StatusCode, "MoMo create payment failed");
            }

            var data = JsonConvert.DeserializeObject<CreatePaymentResponse>(responseContent)!;
            return BaseResponseModel<CreatePaymentResponse>.Success(data);
        }


        public async Task<BaseResponseModel<RefundResponse>> RefundPaymentAsync(RefundRequest model)
        {
            var client = _httpClientFactory.CreateClient();
            string endpoint = _config["MoMo:RefundUrl"]!;
            string partnerCode = _config["MoMo:PartnerCode"]!;
            string accessKey = _config["MoMo:AccessKey"]!;
            string secretKey = _config["MoMo:SecretKey"]!;
            string requestType = "refundMoMoWallet";

            string rawHash = $"accessKey={accessKey}&amount={model.Amount}&orderId={model.OrderId}&partnerCode={partnerCode}&requestId={model.RequestId}&transId={model.TransId}&requestType={requestType}";
            string signature = HmacSHA256(rawHash, secretKey);

            _logger.LogInformation("🔄 [MoMo Refund] RawHash = {rawHash}", rawHash);

            var payload = new
            {
                partnerCode,
                accessKey,
                requestId = model.RequestId,
                amount = model.Amount,
                orderId = model.OrderId,
                transId = model.TransId,
                requestType,
                signature,
                lang = "vi"
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
            };
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.SendAsync(httpRequest);
            var json = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("🔁 [MoMo Refund] Response = {json}", json);

            if (!response.IsSuccessStatusCode)
                return BaseResponseModel<RefundResponse>.Error((int)response.StatusCode, "MoMo refund failed");

            var data = JsonConvert.DeserializeObject<RefundResponse>(json)!;
            return BaseResponseModel<RefundResponse>.Success(data);
        }

        private static string HmacSHA256(string message, string secretKey)
        {
            var encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secretKey);
            byte[] messageBytes = encoding.GetBytes(message);
            using var hmacsha256 = new HMACSHA256(keyByte);
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            return BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
        }
    }
}