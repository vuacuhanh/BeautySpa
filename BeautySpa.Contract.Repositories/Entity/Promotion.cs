using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Promotion : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;

        public Guid ProviderId { get; set; }
        public virtual ApplicationUsers Provider { get; set; }

        public Guid? RequiredRankId { get; set; } 
        public virtual Rank? RequiredRank { get; set; }

        public virtual ICollection<ServicePromotion> ServicePromotions { get; set; } = new List<ServicePromotion>();
    }
}