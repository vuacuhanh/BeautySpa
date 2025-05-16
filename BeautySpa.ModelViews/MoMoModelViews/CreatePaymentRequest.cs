namespace BeautySpa.ModelViews.MoMoModelViews
{
    public class CreatePaymentRequest
    {
        public string OrderId { get; set; } = string.Empty;
        public string OrderInfo { get; set; } = string.Empty;
        public long Amount { get; set; }
    }
}
