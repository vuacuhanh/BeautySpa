using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class ServicePromotion : BaseEntity
    {
        [Required]
        [ForeignKey("Promotion")]
        public string PromotionId { get; set; }

        [Required]
        [ForeignKey("Service")]
        public string ServiceId { get; set; }

        public virtual Promotion Promotion { get; set; }
        public virtual Service Service { get; set; }
    }
}