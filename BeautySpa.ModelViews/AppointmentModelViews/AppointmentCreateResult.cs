namespace BeautySpa.ModelViews.AppointmentModelViews
{
    public class AppointmentCreatedResult
    {
        public Guid AppointmentId { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PayUrl { get; set; }
        public string? QrCodeUrl { get; set; }
    }

}