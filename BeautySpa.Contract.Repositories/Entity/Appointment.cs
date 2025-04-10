using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Appointment : BaseEntity
    {
        [Required]
        [ForeignKey("Customer")]
        public string CustomerId { get; set; }

        [Required]
        [ForeignKey("ServiceProvider")]
        public string ProviderId { get; set; }

        [Required]
        [ForeignKey("Service")]
        public string ServiceId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "pending";

        public string Notes { get; set; }

        public virtual Customers Customer { get; set; }
        public virtual ServiceProvider ServiceProvider { get; set; }
        public virtual Service Service { get; set; }
    }
}