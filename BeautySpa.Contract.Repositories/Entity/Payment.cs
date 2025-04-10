using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Payment : BaseEntity
    {
        [Required]
        [ForeignKey("Appointment")]
        public string AppointmentId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(20)]
        public string PaymentMethod { get; set; }

        [StringLength(100)]
        public string TransactionId { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "pending";

        public DateTime? PaymentDate { get; set; }

        public virtual Appointment Appointment { get; set; }
    }
}