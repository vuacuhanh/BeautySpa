namespace BeautySpa.ModelViews.PromotionModelViews
{
    public class POSTPromotionModelViews
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? DiscountPercent { get; set; }
        //public decimal? DiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid ProviderId { get; set; }
    }
}
