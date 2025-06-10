namespace BeautySpa.ModelViews.PayPalModelViews
{
    public class CreatePayPalPaymentRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Description { get; set; } = "BeautySpa Booking";
        public string? ReturnUrl { get; set; }
        public string? CancelUrl { get; set; }
    }
}
