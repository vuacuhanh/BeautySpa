using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Core.Base;

public class Appointment : BaseEntity
{
    public DateTime AppointmentDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public string BookingStatus { get; set; } = "pending";
    public string? Notes { get; set; }

    public decimal DepositAmount { get; set; } = 0;
    public bool IsDeposited { get; set; } = false;
    public bool IsConfirmedBySpa { get; set; } = false;
    public DateTime? ConfirmationTime { get; set; }
    public int SlotUsed { get; set; } = 1;

    public Guid CustomerId { get; set; }
    public virtual ApplicationUsers? Customer { get; set; }

    public Guid ProviderId { get; set; }
    public virtual ApplicationUsers? Provider { get; set; }

    public Guid ServiceId { get; set; }
    public virtual Service? Service { get; set; }

    public Guid LocationSpaId { get; set; }
    public virtual LocationSpa? LocationSpa { get; set; }

    public virtual Payment? Payment { get; set; }
    public virtual Review? Review { get; set; }

    public Guid? PromotionId { get; set; }
    public virtual Promotion? Promotion { get; set; }

    public Guid? PromotionAdminId { get; set; }
    public virtual PromotionAdmin? PromotionAdmin { get; set; }
}
