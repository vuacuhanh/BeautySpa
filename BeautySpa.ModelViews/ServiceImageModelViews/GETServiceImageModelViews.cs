using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.ServiceImageModelViews
{
    public class GETServiceImageModelViews
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public Guid ServiceProviderId { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
    }
}
