using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Payment : BaseEntity
    {
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public string Status { get; set; } = "pending";
        public DateTime? PaymentDate { get; set; }

        // Khóa ngoại
        public Guid AppointmentId { get; set; }
        public virtual Appointment Appointment { get; set; }
    }
}