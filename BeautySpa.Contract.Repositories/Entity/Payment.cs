using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Payment : BaseEntity
    {
        public decimal Amount { get; set; }                         // Tổng tiền thanh toán (thường là cọc)
        public string PaymentMethod { get; set; } = "Momo";         // Momo, VNPAY, Stripe, etc.
        public string? TransactionId { get; set; }                  // Mã giao dịch MoMo (string để đồng bộ)
        public string Status { get; set; } = "deposit_paid";        // deposit_paid, refunded, completed, failed
        public DateTime? PaymentDate { get; set; }                  // Ngày thanh toán thành công
        public string TransactionType { get; set; } = "Deposit";    // Deposit, Refund, FullPayment

        public decimal RefundAmount { get; set; } = 0;              // Số tiền hoàn lại
        public decimal PlatformFee { get; set; } = 0;               // Phí hệ thống giữ lại (nếu no-show/hủy trễ)

        public Guid AppointmentId { get; set; }
        public virtual Appointment? Appointment { get; set; }
    }
}