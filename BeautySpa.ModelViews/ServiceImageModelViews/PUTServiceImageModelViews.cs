using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.ServiceImageModelViews
{
    public class PUTServiceImageModelViews
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public Guid ServiceProviderId { get; set; }
    }
}
