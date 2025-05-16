using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class RequestBecomeProvider : BaseEntity
    {
        public string BusinessName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string WebsiteOrSocialLink { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? DescriptionImages { get; set; } // chuỗi "url1|url2"
        public decimal? BasePrice { get; set; }

        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }

        public int? ProvinceId { get; set; }
        public string? ProvinceName { get; set; }
        public int? DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public string? AddressDetail { get; set; }

        public string? ServiceCategoryIds { get; set; } // "id1|id2"
        public string? RejectedReason { get; set; }

        public Guid UserId { get; set; }
        public string RequestStatus { get; set; } = "pending";

        public virtual ApplicationUsers? User { get; set; }

    }
}