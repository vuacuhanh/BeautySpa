using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class AdminStaff : BaseEntity
    {
        public string StaffRole { get; set; } = "Support"; // Support, Manager, Approver
        public string Permissions { get; set; } = ""; // Ví dụ: ["ManageUsers","ManageRequests","ManageReports"]

        public bool IsActive { get; set; } = true;

        public Guid AdminId { get; set; }
        public virtual ApplicationUsers? Admin { get; set; }

        public Guid StaffUserId { get; set; }
        public virtual ApplicationUsers? StaffUser { get; set; }
    }
}