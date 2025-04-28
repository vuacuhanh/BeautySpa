using BeautySpa.Core.Base;

public class Payment : BaseEntity
{
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "Momo"; // Momo, VNPAY, Stripe, PayPal
    public string? TransactionId { get; set; }
    public string Status { get; set; } = "pending"; // pending, success, failed, refunded
    public DateTime? PaymentDate { get; set; }
    public string TransactionType { get; set; } = "Deposit"; // Deposit, FullPayment, Refund

    public Guid AppointmentId { get; set; }
    public virtual Appointment? Appointment { get; set; }
}
