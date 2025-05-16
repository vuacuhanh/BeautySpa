namespace BeautySpa.ModelViews.MoMoModelViews
{
    public class RefundRequest
    {
        public string OrderId { get; set; } = string.Empty;
        public string RequestId { get; set; } = string.Empty;
        public long Amount { get; set; }
        public long TransId { get; set; }
        public string Description { get; set; } = string.Empty;
    }

}
