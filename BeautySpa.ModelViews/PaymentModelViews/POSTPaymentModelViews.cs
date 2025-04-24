namespace BeautySpa.ModelViews.PaymentModelViews
{
    public class POSTPaymentModelViews
    {
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime? PaymentDate { get; set; }
        public Guid AppointmentId { get; set; }
    }
}
