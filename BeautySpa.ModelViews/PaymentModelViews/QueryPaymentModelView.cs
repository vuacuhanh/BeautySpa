namespace BeautySpa.ModelViews.PaymentModelViews
{
    public class QueryVnPayModel
    {
        public string TransactionId { get; set; } = string.Empty;

        // Format required by VNPAY: yyyyMMddHHmmss
        public DateTime TransactionDate { get; set; }
    }

    public class QueryMoMoModel
    {
        public string OrderId { get; set; } = string.Empty;
        public string RequestId { get; set; } = string.Empty;

        // MoMo expects TransId to be long
        public long TransId { get; set; }
    }
}
