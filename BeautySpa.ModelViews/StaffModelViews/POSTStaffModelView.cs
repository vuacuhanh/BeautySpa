using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.StaffModelViews
{
    public class POSTStaffModelView
    {
        public Guid? ProviderId { get; set; } // null nếu là beautyspa staff
        public Guid StaffUserId { get; set; }
        public string StaffRole { get; set; } = "Staff";
        public List<string> Permissions { get; set; } = new(); // Convert về JSON khi lưu
    }
}
