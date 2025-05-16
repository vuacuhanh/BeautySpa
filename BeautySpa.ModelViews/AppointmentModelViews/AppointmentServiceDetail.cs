namespace BeautySpa.ModelViews.AppointmentModelViews
{
    public class AppointmentServiceDetail
    {
        public Guid ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public decimal PriceAtBooking { get; set; }
        public int Quantity { get; set; }
    }
}
