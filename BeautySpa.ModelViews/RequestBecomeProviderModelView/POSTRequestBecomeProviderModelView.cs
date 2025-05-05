
namespace BeautySpa.ModelViews.RequestBecomeProviderModelView
{
    public class POSTRequestBecomeProviderModelView
    {
        public string? BusinessName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? WebsiteOrSocialLink { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public List<string>? DescriptionImages { get; set; }
        public decimal? BasePrice { get; set; }

        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }

        public string? AddressDetail { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }

        public List<Guid> ServiceCategoryIds { get; set; } = new();
    }
}
