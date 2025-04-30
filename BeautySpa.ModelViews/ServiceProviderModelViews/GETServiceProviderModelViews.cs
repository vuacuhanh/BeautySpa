using BeautySpa.ModelViews.ServiceImageModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.ServiceProviderModelViews
{
    public class GETServiceProviderModelViews
    {
        public Guid Id { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string BusinessAddress { get; set; } = string.Empty;
        public string WebsiteOrSocialLink { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty; 
        public string BusinessType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ContactFullName { get; set; } = string.Empty;
        public string ContactPosition { get; set; } = string.Empty;
        public int? YearsOfExperience { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public bool IsApproved { get; set; }
        public string Status { get; set; } = "pending";
        public Guid ProviderId { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
        public List<GETServiceImageModelViews>? Images { get; set; } 
    }
}
