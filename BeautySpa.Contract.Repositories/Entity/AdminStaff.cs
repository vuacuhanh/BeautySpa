using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
<<<<<<< HEAD
    public class AdminStaff : BaseEntity
    {
        public string StaffRole { get; set; } = "Support"; // Support, Manager, Approver
        public string Permissions { get; set; } = ""; // Ví dụ: ["ManageUsers","ManageRequests","ManageReports"]
=======
    public string StaffRole { get; set; } = "Support"; 
    public string Permissions { get; set; } = ""; 
>>>>>>> 69164a2c523de3343cce845b080745ed3f4d99bd

        public bool IsActive { get; set; } = true;

        public Guid AdminId { get; set; }
        public virtual ApplicationUsers? Admin { get; set; }

        public Guid StaffUserId { get; set; }
        public virtual ApplicationUsers? StaffUser { get; set; }
    }
}