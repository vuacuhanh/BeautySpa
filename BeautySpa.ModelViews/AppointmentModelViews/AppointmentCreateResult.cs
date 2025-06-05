namespace BeautySpa.ModelViews.AppointmentModelViews
{
    public class AppointmentCreateResult
    {
        public Guid AppointmentId { get; set; }
        public string? PayUrl { get; set; }
        public string? QrCodeUrl { get; set; }
    }
}