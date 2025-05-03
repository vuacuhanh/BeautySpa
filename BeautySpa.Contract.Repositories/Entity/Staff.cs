using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Core.Base;

public class Staff : BaseEntity
{
    public Guid ProviderId { get; set; }
    public virtual ApplicationUsers? Provider { get; set; }

    public Guid StaffUserId { get; set; }
    public virtual ApplicationUsers? StaffUser { get; set; }

    public string StaffRole { get; set; } = "Staff"; // Staff, Manager
    public string Permissions { get; set; } = ""; // Ví dụ: ["ManageServices","ManageAppointments"]

    public bool IsActive { get; set; } = true;
}
