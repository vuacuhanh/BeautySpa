using Newtonsoft.Json;

namespace BeautySpa.ModelViews.MoMoModelViews
{
    public class CreatePaymentResponse
    {
        public string? PartnerCode { get; set; }
        public string? OrderId { get; set; }
        public string? RequestId { get; set; }
        public int Amount { get; set; }
        public string? ResponseTime { get; set; }
        public string? Message { get; set; }
        public int ResultCode { get; set; }
        public string? PayUrl { get; set; }
        public string? Deeplink { get; set; }
        public string? QrCodeUrl { get; set; }
        public string? ErrorCode { get; set; }
    }
}
