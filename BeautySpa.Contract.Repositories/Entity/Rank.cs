using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Rank: BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int MinPoints { get; set; }
        public decimal? DiscountPercent { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<MemberShip> MemberShips { get; set; } = new List<MemberShip>();
        public virtual ICollection<PromotionAdminRank> PromotionAdminRanks { get; set; } = new List<PromotionAdminRank>();
    }
}
