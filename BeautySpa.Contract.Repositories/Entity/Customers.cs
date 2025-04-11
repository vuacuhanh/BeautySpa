using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Customers : BaseEntity
    {
        public Guid UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? AvatarUrl { get; set; }

        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public bool IsVerified { get; set; } = false;

        public string Status { get; set; } = "active";

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUsers User { get; set; } = null!;

        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    }
}