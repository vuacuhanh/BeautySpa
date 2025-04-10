using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Review : BaseEntity
    {
        [Required]
        [ForeignKey("Appointment")]
        public string AppointmentId { get; set; }

        [Required]
        [ForeignKey("Customer")]
        public string CustomerId { get; set; }

        [Required]
        [ForeignKey("ServiceProvider")]
        public string ProviderId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; }

        public string ProviderResponse { get; set; }

        public virtual Appointment Appointment { get; set; }
        public virtual Customers Customer { get; set; }
        public virtual ServiceProvider ServiceProvider { get; set; }
    }
}