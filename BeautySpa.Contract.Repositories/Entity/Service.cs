using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Service : BaseEntity
    {
        public string ServiceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public decimal? DiscountPrice { get; set; }
        public bool IsAvailable { get; set; } = true;

        public Guid ProviderId { get; set; }
        public virtual ApplicationUsers? Provider { get; set; }

        public Guid CategoryId { get; set; }
        public virtual ServiceCategory? Category { get; set; }

        public virtual ICollection<ServicePromotion> ServicePromotions { get; set; } = new List<ServicePromotion>();
        public virtual ICollection<ServiceImage> ServiceImages { get; set; } = new List<ServiceImage>();
    }
}