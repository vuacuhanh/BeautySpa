using System.Security.Cryptography;
using System.Text;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.MoMoModelViews;
using BeautySpa.Services.Validations.MomoValidator;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BeautySpa.Services.Service
{
    public class MomoService : IMomoService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public MomoService(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<BaseResponseModel<CreatePaymentResponse>> CreatePaymentAsync(CreatePaymentRequest model)
        {
            await new CreatePaymentValidator().ValidateAndThrowAsync(model);

            string endpoint = GetConfig("MoMo:PaymentUrl");
            string partnerCode = GetConfig("MoMo:PartnerCode");
            string accessKey = GetConfig("MoMo:AccessKey");
            string secretKey = GetConfig("MoMo:SecretKey");
            string redirectUrl = _config["MoMo:RedirectUrl"] ?? "";
            string ipnUrl = _config["MoMo:IpnUrl"] ?? "";

            string requestId = Guid.NewGuid().ToString();
            string rawHash = $"accessKey={accessKey}&amount={model.Amount}&extraData=&ipnUrl={ipnUrl}&orderId={model.OrderId}&orderInfo={model.OrderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType=captureWallet";

            string signature = ComputeHmacSha256(rawHash, secretKey);

            var body = new
            {
                partnerCode,
                accessKey,
                requestId,
                amount = model.Amount.ToString(),
                orderId = model.OrderId,
                orderInfo = model.OrderInfo,
                redirectUrl,
                ipnUrl,
                extraData = "",
                requestType = "captureWallet",
                signature,
                lang = "vi"
            };

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);
            string result = await response.Content.ReadAsStringAsync();

            dynamic json = JsonConvert.DeserializeObject(result)
                ?? throw new ErrorException(500, ErrorCode.InternalServerError, "MoMo response is null");

            var responseModel = new CreatePaymentResponse
            {
                DeeplinkQr = json.deeplink ?? "",
                PayUrl = json.payUrl ?? "",
                RequestId = requestId
            };

            return BaseResponseModel<CreatePaymentResponse>.Success(responseModel);
        }

        public async Task<BaseResponseModel<RefundResponse>> RefundPaymentAsync(RefundRequest model)
        {
            await new RefundValidator().ValidateAndThrowAsync(model);

            string endpoint = GetConfig("MoMo:RefundUrl");
            string partnerCode = GetConfig("MoMo:PartnerCode");
            string accessKey = GetConfig("MoMo:AccessKey");
            string secretKey = GetConfig("MoMo:SecretKey");

            string rawHash = $"accessKey={accessKey}&amount={model.Amount}&description={model.Description}&orderId={model.OrderId}&partnerCode={partnerCode}&requestId={model.RequestId}&transId={model.TransId}";
            string signature = ComputeHmacSha256(rawHash, secretKey);

            var body = new
            {
                partnerCode,
                accessKey,
                requestId = model.RequestId,
                amount = model.Amount.ToString(),
                orderId = model.OrderId,
                transId = model.TransId.ToString(),
                lang = "vi",
                description = model.Description,
                signature
            };

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);
            string result = await response.Content.ReadAsStringAsync();

            dynamic json = JsonConvert.DeserializeObject(result)
                ?? throw new ErrorException(500, ErrorCode.InternalServerError, "MoMo refund response is null");

            var responseModel = new RefundResponse
            {
                Message = json.message ?? "",
                ResultCode = json.resultCode ?? ""
            };

            return BaseResponseModel<RefundResponse>.Success(responseModel);
        }

        private string GetConfig(string key)
        {
            return _config[key] ?? throw new ErrorException(500, ErrorCode.InternalServerError, $"Missing config key: {key}");
        }

        private static string ComputeHmacSha256(string rawData, string secretKey)
        {
            if (string.IsNullOrEmpty(secretKey))
                throw new ErrorException(500, ErrorCode.InternalServerError, "Secret key is empty");

            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
