using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Review : BaseEntity
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? ProviderResponse { get; set; }

        // Khóa ngoại
        public Guid AppointmentId { get; set; }
        public virtual Appointment Appointment { get; set; }

        public Guid CustomerId { get; set; }
        public virtual ApplicationUsers Customer { get; set; }

        public Guid ProviderId { get; set; }
        public virtual ApplicationUsers Provider { get; set; }
    }
}