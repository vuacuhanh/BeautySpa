namespace BeautySpa.ModelViews.PaymentModelViews
{
    public class GETPaymentModelView
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public decimal Amount { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal PlatformFee { get; set; }
        public string Status { get; set; } = string.Empty; // deposit_paid | refunded | completed
        public string PaymentMethod { get; set; } = string.Empty; // Momo | Vnpay
        public string? TransactionId { get; set; }
        public DateTimeOffset? PaymentDate { get; set; }
    }
}
