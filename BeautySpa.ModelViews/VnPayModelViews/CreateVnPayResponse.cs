namespace BeautySpa.ModelViews.VnPayModelViews
{
    public class CreateVnPayResponse
    {
        public string PayUrl { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string Message { get; set; } = "VNPAY payment URL created";
    }

}
