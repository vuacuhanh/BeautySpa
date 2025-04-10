using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Notification : BaseEntity
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(10)]
        public string UserType { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        [StringLength(20)]
        public string NotificationType { get; set; }

        public bool IsRead { get; set; } = false;
    }
}