using System.ComponentModel.DataAnnotations;

namespace BeautySpa.ModelViews.AppointmentModelViews
{
    public class POSTAppointmentModelViews
    {

        public required DateTime AppointmentDate { get; set; }
        public required TimeSpan StartTime { get; set; }

        public string Status { get; set; } = "Pending";
        public string? Notes { get; set; }

        public Guid CustomerId { get; set; }
        public Guid ProviderId { get; set; }
        public Guid ServiceId { get; set; }
        public Guid LocationSpaId { get; set; }
    }
}
