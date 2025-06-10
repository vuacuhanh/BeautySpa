namespace BeautySpa.ModelViews.MoMoModelViews
{
    public class RefundRequest
    {
        public string OrderId { get; set; } = default!;
        public int Amount { get; set; }
        public string TransId { get; set; } = default!;
        public string? RequestId { get; set; }
    }
}
