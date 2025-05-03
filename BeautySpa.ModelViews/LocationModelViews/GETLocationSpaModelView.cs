namespace BeautySpa.ModelViews.LocationModelViews
{
    public class GETLocationSpaModelView
    {
        public Guid Id { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? Description { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public bool IsActive { get; set; }

        public Guid BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
    }
}
