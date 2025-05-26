using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class AppointmentService : BaseEntity
    {
        public Guid AppointmentId { get; set; }
        public virtual Appointment? Appointment { get; set; }

        public Guid ServiceId { get; set; }
        public virtual Service? Service { get; set; }

        public decimal PriceAtBooking { get; set; } 
        public int Quantity { get; set; } = 1;
    }
}
