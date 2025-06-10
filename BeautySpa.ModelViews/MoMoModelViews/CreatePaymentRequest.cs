namespace BeautySpa.ModelViews.MoMoModelViews
{
    public class CreatePaymentRequest
    {
        public string OrderId { get; set; } = default!;
        public string OrderInfo { get; set; } = default!;
        public int Amount { get; set; } // VND
        public string? ExtraData { get; set; } = "";
        public string? RequestType { get; set; } = "captureMoMoWallet";
    }
}
