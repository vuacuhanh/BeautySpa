using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class PromotionAdminRank : BaseEntity
    {
        public Guid PromotionAdminId { get; set; }
        public virtual PromotionAdmin PromotionAdmin { get; set; } = new PromotionAdmin();

        public Guid RankId { get; set; }
        public virtual Rank? Rank { get; set; }

        public int MaxUsePerUser { get; set; } = 1;
    }
}
