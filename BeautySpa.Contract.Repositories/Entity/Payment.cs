using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Payment : BaseEntity
    {
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "Momo"; // Momo, VNPAY, Stripe, PayPal
        public string? TransactionId { get; set; }
        public string Status { get; set; } = "pending"; // pending, success, refunded, failed
        public DateTime? PaymentDate { get; set; }
        public string TransactionType { get; set; } = "FullPayment"; // FullPayment, Refund

        public decimal RefundAmount { get; set; } = 0;      // Nếu bị hoàn tiền
        public decimal PlatformFee { get; set; } = 0;       // Phí trừ lại 2%

        public Guid AppointmentId { get; set; }
        public virtual Appointment? Appointment { get; set; }
    }
}