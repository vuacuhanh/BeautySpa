using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class ServiceImage : BaseEntity
    {
        [Required]
        [ForeignKey("Service")]
        public string ServiceId { get; set; }

        [Required]
        [StringLength(255)]
        public string ImageUrl { get; set; }

        public bool IsPrimary { get; set; } = false;

        public virtual Service Service { get; set; }
    }
}