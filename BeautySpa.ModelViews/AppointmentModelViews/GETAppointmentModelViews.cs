namespace BeautySpa.ModelViews.AppointmentModelViews
{
    public class GETAppointmentModelView
    {
        public Guid Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public string? StaffName { get; set; }
        public string BookingStatus { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public decimal OriginalTotalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public List<AppointmentServiceDetail> Services { get; set; } = new();

        public string BranchName { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public string ProvinceName { get; set; } = string.Empty;
    }
}

