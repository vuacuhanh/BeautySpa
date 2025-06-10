namespace BeautySpa.Core.Settings
{
    public class PayPalSettings
    {
        public string ClientId { get; set; } = null!;
        public string Secret { get; set; } = null!;
        public string BaseUrl { get; set; } = "https://api-m.sandbox.paypal.com";
        public string ReturnUrl { get; set; } = null!;
        public string CancelUrl { get; set; } = null!;
    }
}
