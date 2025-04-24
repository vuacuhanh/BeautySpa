namespace BeautySpa.ModelViews.ReviewModelViews
{
    public class PUTReviewModelViews
    {
        public Guid Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? ProviderResponse { get; set; }
    }
}
