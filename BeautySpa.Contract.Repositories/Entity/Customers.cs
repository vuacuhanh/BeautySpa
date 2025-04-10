using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Customers : BaseEntity
    {
        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(255)]
        public string AvatarUrl { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public bool IsVerified { get; set; } = false;

        [StringLength(10)]
        public string Status { get; set; } = "active";
    }
}