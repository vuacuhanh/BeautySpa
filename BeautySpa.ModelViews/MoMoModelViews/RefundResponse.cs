using Newtonsoft.Json;

namespace BeautySpa.ModelViews.MoMoModelViews
{
    public class RefundResponse
    {
        public string? OrderId { get; set; }
        public string? RequestId { get; set; }
        public string? Message { get; set; }
        public string? ResultCode { get; set; }
        public int Amount { get; set; }
        public string? TransId { get; set; }
    }
}
