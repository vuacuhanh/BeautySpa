namespace BeautySpa.ModelViews.PaymentModelViews
{
    public class POSTPaymentModelView
    {
        public Guid AppointmentId { get; set; }
        public int Amount { get; set; } // ✅ dùng long để đồng bộ MoMo
        public string PaymentMethod { get; set; } = "Momo"; // Momo | Vnpay
    }
}
