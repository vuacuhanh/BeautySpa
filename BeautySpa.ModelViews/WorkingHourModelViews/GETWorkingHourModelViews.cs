namespace BeautySpa.ModelViews.WorkingHourModelViews
{
    public class GETWorkingHourModelViews
    {
        public Guid Id { get; set; }
        public int DayOfWeek { get; set; } // 0: Chủ nhật, 1: Thứ 2, ...
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public bool IsWorking { get; set; }
        public Guid ProviderId { get; set; }

        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset? LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
    }
}
