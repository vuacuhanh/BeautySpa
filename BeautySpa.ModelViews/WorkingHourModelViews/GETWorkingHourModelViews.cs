namespace BeautySpa.ModelViews.WorkingHourModelViews
{
    public class GETWorkingHourModelViews
    {
        public Guid Id { get; set; }
        public int DayOfWeek { get; set; } 
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public bool IsWorking { get; set; }
        public Guid? ProviderId { get; set; } = null;
        public Guid? SpaBranchLocationId { get; set; } = null;
        public string? BranchName { get; set; } = null;
    }
}