namespace BeautySpa.ModelViews.AppointmentModelViews
{
    public class POSTAppointmentModelViews
    {
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Notes { get; set; }

        public Guid CustomerId { get; set; }
        public Guid ProviderId { get; set; }
        public Guid ServiceId { get; set; }
    }
}
