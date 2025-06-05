namespace BeautySpa.Core.Settings
{
    public class MomoSettings
    {
        public string PaymentUrl { get; set; } = string.Empty;
        public string RefundUrl { get; set; } = string.Empty;   
        public string PartnerCode { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string NotifyUrl { get; set; } = string.Empty;
        public string RequestType { get; set; } = "captureMoMoWallet";
    }
}