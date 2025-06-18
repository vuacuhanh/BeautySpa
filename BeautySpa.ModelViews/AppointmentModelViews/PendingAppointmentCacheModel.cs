using BeautySpa.Contract.Repositories.Entity;

namespace BeautySpa.ModelViews.AppointmentModelViews
{
    public class PendingAppointmentCacheModel
    {
        public Guid Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public Guid SpaBranchLocationId { get; set; }
        public string? Notes { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ProviderId { get; set; }
        public Guid? PromotionId { get; set; }
        public Guid? PromotionAdminId { get; set; }
        public decimal OriginalTotalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public List<AppointmentService> Services { get; set; } = new();
        public string? CreatedBy { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public string BookingStatus { get; set; } = "waiting_payment";
    }
}
