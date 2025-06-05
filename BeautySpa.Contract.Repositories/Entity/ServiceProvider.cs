using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class ServiceProvider : BaseEntity
    {
        public string BusinessName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ContactFullName { get; set; } = string.Empty;
        public string ContactPosition { get; set; } = string.Empty;
        public int? YearsOfExperience { get; set; }
        public decimal AverageRating { get; set; } = 0.00m;
        public int TotalReviews { get; set; } = 0;
        public bool IsApproved { get; set; } = false;
        public string Status { get; set; } = "pending";
        public string PhoneNumber { get; set; } = string.Empty; 
        public string? Email { get; set; }

        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public int MaxAppointmentsPerSlot { get; set; } = 5;

        public Guid ProviderId { get; set; }
        public virtual ApplicationUsers? Provider { get; set; }

        public virtual ICollection<ServiceProviderCategory> ServiceProviderCategories { get; set; } = new List<ServiceProviderCategory>();
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
        public virtual ICollection<ServiceImage> ServiceImages { get; set; } = new List<ServiceImage>();
        public virtual ICollection<WorkingHour> WorkingHours { get; set; } = new List<WorkingHour>();

    }
}