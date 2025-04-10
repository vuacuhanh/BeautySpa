using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class ServiceProvider : BaseEntity
    {
        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(100)]
        public string BusinessName { get; set; }

        [Required]
        [StringLength(255)]
        public string BusinessAddress { get; set; }

        [Required]
        [StringLength(255)]
        public string WebsiteOrSocialLink { get; set; }

        [Required]
        [StringLength(50)]
        public string BusinessType { get; set; }

        public string Description { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string ContactFullName { get; set; }

        [Required]
        [StringLength(50)]
        public string ContactPosition { get; set; }

        public int? YearsOfExperience { get; set; }

        public decimal AverageRating { get; set; } = 0.00m;

        public int TotalReviews { get; set; } = 0;

        public bool IsApproved { get; set; } = false;

        [StringLength(10)]
        public string Status { get; set; } = "pending";
    }
}