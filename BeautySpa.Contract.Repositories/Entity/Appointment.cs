using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Appointment : BaseEntity
    {
        public required DateTime AppointmentDate { get; set; }
        public required TimeSpan StartTime { get; set; }
        public string Status { get; set; } = "pending";
        public string? Notes { get; set; }

        // Khóa ngoại
        public Guid CustomerId { get; set; }
        public virtual ApplicationUsers Customer { get; set; }

        public Guid ProviderId { get; set; }
        public virtual ApplicationUsers Provider { get; set; }

        public Guid ServiceId { get; set; }
        public virtual Service Service { get; set; }

        // Mối quan hệ
        public virtual Payment? Payment { get; set; }
        public virtual Review? Review { get; set; }
    }
}