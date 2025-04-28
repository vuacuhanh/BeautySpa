using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Core.Base;

public class ServiceImage : BaseEntity
{
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = false; // ảnh chính hay phụ

    public Guid ServiceId { get; set; }
    public virtual Service? Service { get; set; }
}
