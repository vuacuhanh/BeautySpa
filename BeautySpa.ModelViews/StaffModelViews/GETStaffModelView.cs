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
        public Guid? ProviderId { get; set; }
        public Guid StaffUserId { get; set; }
        public string StaffRole { get; set; } = "Staff";
        public List<string> Permissions { get; set; } = new();
        public bool IsActive { get; set; }
    }
}
