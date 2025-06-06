namespace BeautySpa.ModelViews.ReviewModelViews
{
    public class POSTReviewModelViews
    {
        public Guid AppointmentId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
