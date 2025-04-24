namespace BeautySpa.ModelViews.WorkingHourModelViews
{
    public class PUTWorkingHourModelViews
    {
        public Guid Id { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public bool IsWorking { get; set; }
    }
}
