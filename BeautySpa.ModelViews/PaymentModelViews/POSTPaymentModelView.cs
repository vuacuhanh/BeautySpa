namespace BeautySpa.ModelViews.PaymentModelViews
{
    public class POSTPaymentModelView 
    {
        public Guid AppointmentId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "Momo"; // "Momo" | "Vnpay"
    }
}
