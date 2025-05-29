using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Core.Base;

public class AdminStaff : BaseEntity
{
    public string StaffRole { get; set; } = "Support"; 
    public string Permissions { get; set; } = ""; 

    public bool IsActive { get; set; } = true;

    public Guid AdminId { get; set; }
    public virtual ApplicationUsers? Admin { get; set; }

    public Guid StaffUserId { get; set; }
    public virtual ApplicationUsers? StaffUser { get; set; }
}
