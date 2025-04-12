using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.UserModelViews
{
    public class GETUserInfoforcustomerModelView
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public DateTime? DayofBirth { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Gender { get; set; }
        public bool IsActive { get; set; }
    }
}
