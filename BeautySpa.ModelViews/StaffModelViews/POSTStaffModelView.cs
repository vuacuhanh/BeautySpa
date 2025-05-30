using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.StaffModelViews
{
    public class POSTStaffModelView
    {
        public Guid BranchId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Gender { get; set; }
        public int? YearsOfExperience { get; set; }
        public List<Guid> ServiceCategoryIds { get; set; } = new();
    }
}
