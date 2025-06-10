namespace BeautySpa.ModelViews.PayPalModelViews
{
    public class RefundPayPalRequest
    {
        public string CaptureId { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
    }
}
