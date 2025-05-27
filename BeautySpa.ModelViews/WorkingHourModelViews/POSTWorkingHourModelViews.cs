namespace BeautySpa.ModelViews.WorkingHourModelViews
{
    public class POSTWorkingHourModelViews
    {
        public int DayOfWeek { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public bool IsWorking { get; set; } = true;
        public Guid? ProviderId { get; set; }
        public Guid? SpaBranchLocationId { get; set; }
    }
}
