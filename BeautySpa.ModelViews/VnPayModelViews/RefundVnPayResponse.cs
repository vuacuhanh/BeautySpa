namespace BeautySpa.ModelViews.VnPayModelViews
{
    public class RefundVnPayResponse
    {
        public int ResponseCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
    }
}