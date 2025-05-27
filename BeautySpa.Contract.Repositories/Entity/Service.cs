using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Service : BaseEntity
    {
        public string ServiceName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int DurationMinutes { get; set; } = 0;
        public string ImageUrl { get; set; } = string.Empty;
        public Guid ProviderId { get; set; }
        public ServiceProvider? Provider { get; set; }
        public Guid ServiceCategoryId { get; set; }
        public ServiceCategory? ServiceCategory { get; set; }
        public ICollection<ServiceImage> ServiceImages { get; set; } = new List<ServiceImage>();
        public ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
        public ICollection<ServicePromotion> ServicePromotions { get; set; } = new List<ServicePromotion>();
    }
}
