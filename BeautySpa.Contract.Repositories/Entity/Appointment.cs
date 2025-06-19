using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Appointment : BaseEntity
    {
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public string BookingStatus { get; set; } = "pending";
        public string? Notes { get; set; }

        public bool IsConfirmedBySpa { get; set; } = false;
        public DateTime? ConfirmationTime { get; set; }
        public int SlotUsed { get; set; } = 1;

        // Tổng tiền & khuyến mãi
        public decimal OriginalTotalPrice { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;
        public decimal FinalPrice { get; set; } = 0;

        public Guid CustomerId { get; set; }
        public virtual ApplicationUsers? Customer { get; set; }

        public Guid ProviderId { get; set; }
        public virtual ApplicationUsers? Provider { get; set; }
        
        public Guid SpaBranchLocationId { get; set; }
        public virtual SpaBranchLocation? BranchLocation { get; set; }

        public Guid? PromotionId { get; set; }
        public virtual Promotion? Promotion { get; set; }

        public Guid? PromotionAdminId { get; set; }
        public virtual PromotionAdmin? PromotionAdmin { get; set; }

        public virtual Payment? Payment { get; set; }
        public virtual Review? Review { get; set; }

        public Guid? StaffId { get; set; }
        public virtual Staff? Staff { get; set; }


        public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
    }
}