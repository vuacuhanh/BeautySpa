using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Core.Base;

public class ServicePromotion : BaseEntity
{
    public decimal DiscountPrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Khóa ngoại
    public Guid ServiceId { get; set; }
    public virtual Service? Service { get; set; }
}
