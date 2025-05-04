using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class ServicePromotion : BaseEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid ServiceId { get; set; }
        public virtual Service? Service { get; set; }

        public Guid PromotionId { get; set; }
        public virtual Promotion? Promotion { get; set; }
    }
}
