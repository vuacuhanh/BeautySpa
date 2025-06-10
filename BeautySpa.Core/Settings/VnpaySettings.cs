namespace BeautySpa.Core.Settings
{
    public class VnpaySettings
    {
        public string PaymentUrl { get; set; } = string.Empty;
        public string RefundUrl { get; set; } = string.Empty;
        public string TmnCode { get; set; } = string.Empty;
        public string HashSecret { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty ;
    }
}
