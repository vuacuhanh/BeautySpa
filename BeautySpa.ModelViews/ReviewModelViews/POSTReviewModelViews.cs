namespace BeautySpa.ModelViews.ReviewModelViews
{
    public class POSTReviewModelViews
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }

        public Guid AppointmentId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ProviderId { get; set; }
    }
}
