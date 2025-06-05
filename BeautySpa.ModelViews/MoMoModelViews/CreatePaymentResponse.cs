using Newtonsoft.Json;

namespace BeautySpa.ModelViews.MoMoModelViews
{
    public class CreatePaymentResponse
    {
        [JsonProperty("requestId")]
        public string RequestId { get; set; } = string.Empty;

        [JsonProperty("resultCode")]
        public int ResultCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;

        [JsonProperty("qrCodeUrl")]
        public string QrCodeUrl { get; set; } = string.Empty;

        [JsonProperty("payUrl")]
        public string PayUrl { get; set; } = string.Empty;
    }
}