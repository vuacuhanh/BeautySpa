namespace BeautySpa.ModelViews.PaymentModelViews
{
    public class GETPaymentModelView
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public int Amount { get; set; }          // ✅ sửa từ decimal → long
        public long RefundAmount { get; set; }    // ✅ sửa từ decimal → long
        public long PlatformFee { get; set; }     // ✅ sửa từ decimal → long
        public string Status { get; set; } = string.Empty; // deposit_paid | refunded | completed
        public string PaymentMethod { get; set; } = string.Empty; // Momo | Vnpay
        public string? TransactionId { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
