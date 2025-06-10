namespace BeautySpa.ModelViews.PaymentModelViews
{
    public class PaymentResponse
    {
        public Guid AppointmentId { get; set; }
        public int Amount { get; set; }          // ✅ long cho đồng bộ
        public string PaymentMethod { get; set; } = string.Empty;
        public string? PayUrl { get; set; }
        public string? QrCodeUrl { get; set; }
    }
}
