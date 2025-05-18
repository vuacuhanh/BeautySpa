using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.UserModelViews
{
    public class GETUserModelViews
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Gender {  get; set; }
        public string? AddressDetail { get; set; }
        public string? ProvinceId { get; set; }
        public string? ProvinceName { get; set; }
        public string? DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}