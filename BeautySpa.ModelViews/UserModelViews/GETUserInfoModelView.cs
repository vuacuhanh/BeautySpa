using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.UserModelViews
{
    public class GETUserInfoModelView
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime? DayofBirth { get; set; }
        public string? AddressDetail { get; set; }
        public string? ProvinceId { get; set; }
        public string? ProvinceName { get; set; }
        public string? DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Gender { get; set; }
        public decimal? Salary { get; set; }
        public string? BankAccount { get; set; }
        public string? BankAccountName { get; set; }
        public string? Bank { get; set; }
    }
}
