using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.ServiceProviderModelViews
{
    public class POSTServiceProviderModelViews
    {
        public string BusinessName { get; set; } = string.Empty;
        public string BusinessAddress { get; set; } = string.Empty;
        public string WebsiteOrSocialLink { get; set; } = string.Empty;
        public string BusinessType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ContactFullName { get; set; } = string.Empty;
        public string ContactPosition { get; set; } = string.Empty;
        public int? YearsOfExperience { get; set; }
        public Guid ProviderId { get; set; }

    }
}
