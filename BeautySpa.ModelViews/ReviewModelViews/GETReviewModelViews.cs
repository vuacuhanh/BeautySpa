namespace BeautySpa.ModelViews.ReviewModelViews
{
    public class GETReviewModelViews
    {
        public Guid Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? ProviderResponse { get; set; }

        public Guid AppointmentId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ProviderId { get; set; }

        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset? LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
    }
}
