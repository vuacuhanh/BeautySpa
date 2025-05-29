using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.StaffModelViews
{
    public class GETStaffModelView
    {
        public Guid Id { get; set; }
        public Guid ProviderId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Gender { get; set; }
        public int? YearsOfExperience { get; set; }
        public List<Guid> ServiceCategoryIds { get; set; } = new();
        public List<string> ServiceCategoryNames { get; set; } = new();
    }
}
