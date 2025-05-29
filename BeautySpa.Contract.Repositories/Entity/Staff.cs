using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Core.Base;

public class Staff : BaseEntity
{
    public Guid ProviderId { get; set; }
    public virtual ServiceProvider? Provider { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Gender { get; set; }
    public int? YearsOfExperience { get; set; }
    public virtual ICollection<StaffServiceCategory> StaffServiceCategories { get; set; } = new List<StaffServiceCategory>();
}
