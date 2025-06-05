using BeautySpa.ModelViews.ServiceImageModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.ServiceModelViews
{
    public class GETServiceModelViews
    {
        public Guid Id { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string? CategoryName { get; set; }
        public Guid ProviderId { get; set; }
        public Guid CategoryId { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
    }
}
