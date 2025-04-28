using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class PromotionAdmin : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;

        public Guid? RankId { get; set; }  
        public virtual Rank? Rank { get; set; }
    }
}
