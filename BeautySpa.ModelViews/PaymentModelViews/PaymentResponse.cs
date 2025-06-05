namespace BeautySpa.ModelViews.PaymentModelViews
{
    public class PaymentResponse
    {
        public Guid AppointmentId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? PayUrl { get; set; }
        public string? QrCodeUrl { get; set; }  
    }
}