using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class ServicePromotion : BaseEntity
    {
        public string PromotionId { get; set; }
        public virtual Promotion Promotion { get; set; }

        public string ServiceId { get; set; }
        public virtual Service Service { get; set; }
    }
}