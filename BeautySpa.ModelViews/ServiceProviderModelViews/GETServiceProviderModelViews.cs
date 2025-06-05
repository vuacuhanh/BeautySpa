using BeautySpa.ModelViews.ServiceImageModelViews;

namespace BeautySpa.ModelViews.ServiceProviderModelViews
{
    public class GETServiceProviderModelViews
    {
        public Guid Id { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ContactFullName { get; set; } = string.Empty;
        public string ContactPosition { get; set; } = string.Empty;
        public int? YearsOfExperience { get; set; }
        public decimal AverageRating { get; set; }
        public int MaxAppointmentsPerSlot { get; set; } = 5;
        public int TotalReviews { get; set; }
        public bool IsApproved { get; set; }
        public string? AddressDetail { get; set; }
        public string? ProvinceId { get; set; }
        public string? DistrictId { get; set; }
        public string? ProvinceName { get; set; }
        public string? DistrictName { get; set; }
        public string Status { get; set; } = "pending";
        public Guid ProviderId { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public List<GETServiceImageModelViews>? Images { get; set; }
        public List<string>? Categories { get; set; }
    }
}