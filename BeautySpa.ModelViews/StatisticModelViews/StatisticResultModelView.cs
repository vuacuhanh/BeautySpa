namespace BeautySpa.ModelViews.StatisticModelViews
{
    public class StatisticResultModelView
    {
        public int TotalAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
        public int NoShowCount { get; set; }
        public List<TopServiceModel> TopServicesToday { get; set; } = new();
        public List<TopServiceModel> TopServicesWeek { get; set; } = new();
        public List<TopServiceModel> TopServicesYear { get; set; } = new();
        public List<MonthlyRevenueModel> RevenueByMonth { get; set; } = new();

        // Admin only
        public decimal TotalCommissionRevenue { get; set; }
        public int ApprovedProviderCount { get; set; }
        public List<TopProviderModel> TopBookedProviders { get; set; } = new();
    }

    public class TopServiceModel
    {
        public string? ServiceName { get; set; }
        public int TotalBooked { get; set; }
    }

    public class MonthlyRevenueModel
    {
        public string? Month { get; set; } // Format: yyyy-MM
        public decimal Revenue { get; set; }
    }

    public class TopProviderModel
    {
        public string? ProviderName { get; set; }
        public int TotalAppointments { get; set; }
    }
}
