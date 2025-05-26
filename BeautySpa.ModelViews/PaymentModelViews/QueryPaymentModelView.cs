namespace BeautySpa.ModelViews.PaymentModelViews
{
    public class QueryVnPayModel
    {
        public string TransactionId { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; } // VNPAY yêu cầu yyyyMMddHHmmss
    }

    public class QueryMoMoModel
    {
        public string OrderId { get; set; } = string.Empty;
        public string RequestId { get; set; } = string.Empty;
        public long TransId { get; set; }
    }
}
