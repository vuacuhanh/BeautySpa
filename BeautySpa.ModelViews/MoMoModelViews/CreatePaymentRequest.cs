namespace BeautySpa.ModelViews.MoMoModelViews
{
    public class CreatePaymentRequest
    {
        public string OrderId { get; set; } = string.Empty;
        public string OrderInfo { get; set; } = string.Empty;
        public long Amount { get; set; }
        public string? RequestId { get; set; }
        public string RedirectUrl { get; set; } = string.Empty;
        public string ExtraData { get; set; } = string.Empty;
    }
}
