using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class ServiceProvider : BaseEntity
    {
        public string BusinessName { get; set; } = string.Empty;
        public string WebsiteOrSocialLink { get; set; } = string.Empty;
        public string BusinessType { get; set; } = string.Empty;
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
        // Khóa ngoại
        public Guid ProviderId { get; set; }
        public virtual ApplicationUsers Provider { get; set; }

    }
}