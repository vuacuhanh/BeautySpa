using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Promotion : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? DiscountPercent { get; set; }    // Giảm theo %
        public decimal? DiscountAmount { get; set; }     // Giảm theo số tiền
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;

        public Guid ProviderId { get; set; }
        public virtual ApplicationUsers? Provider { get; set; }

        public virtual ICollection<ServicePromotion> ServicePromotions { get; set; } = new List<ServicePromotion>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
    