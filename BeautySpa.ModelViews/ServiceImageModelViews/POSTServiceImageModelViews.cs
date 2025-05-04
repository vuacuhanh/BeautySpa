using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.ServiceImageModelViews
{
    public class POSTServiceImageModelViews
    {
        public Guid ServiceProviderId { get; set; }
        public List<string> ImageUrls { get; set; } = new();
    }
}
