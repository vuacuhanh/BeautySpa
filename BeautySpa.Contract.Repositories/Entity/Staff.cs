using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Core.Base;

public class Staff : BaseEntity
{
    public Guid ProviderId { get; set; }
    public virtual ApplicationUsers? Provider { get; set; }

    public Guid StaffUserId { get; set; }
    public virtual ApplicationUsers? StaffUser { get; set; }

    public string StaffRole { get; set; } = "Staff";

    public string Permissions { get; set; } = ""; 

    public bool IsActive { get; set; } = true;
}
