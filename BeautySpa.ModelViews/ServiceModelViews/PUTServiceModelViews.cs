using BeautySpa.ModelViews.ServiceImageModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.ServiceModelViews
{
    public class PUTServiceModelViews
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; } =  0;
        //public Guid ProviderId { get; set; }
        public Guid CategoryId { get; set; }
    }
}
