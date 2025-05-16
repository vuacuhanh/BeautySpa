namespace BeautySpa.ModelViews.PaymentModelViews
{
    public class RefundPaymentModelView
    {
        public Guid AppointmentId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public bool KeepPlatformFee { get; set; } = true; // true = trừ 10%
    }
}
