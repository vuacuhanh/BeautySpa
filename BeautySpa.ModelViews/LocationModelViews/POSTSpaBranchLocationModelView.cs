namespace BeautySpa.ModelViews.LocationModelViews
{
    public class POSTSpaBranchLocationModelView
    {
        public Guid ServiceProviderId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = "Vietnam";
        public string? ProvinceId { get; set; }
        public string? DistrictId { get; set; }
    }
}
