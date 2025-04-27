using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.ServiceImageModelViews
{
    public class POSTServiceImageModelViews
    {
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;
        public Guid ServiceProviderId { get; set; }
    }
}
