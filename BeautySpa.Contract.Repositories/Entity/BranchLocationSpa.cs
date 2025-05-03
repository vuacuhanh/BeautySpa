using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class BranchLocationSpa : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset? DisabledTime { get; set; }
        public string? DeactivatedNote { get; set; }

        // Mối quan hệ
        public virtual ICollection<LocationSpa> LocationSpas { get; set; } = new List<LocationSpa>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
