using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.RequestBecomeProviderModelView
{
    public class RegisterRequestBecomeProviderModelView
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public string ContactPosition { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? AddressDetail { get; set; }
        public string? ProvinceId { get; set; }
        public string? DistrictId { get; set; }
        public List<Guid> ServiceCategoryIds { get; set; } = new();
    }
}
