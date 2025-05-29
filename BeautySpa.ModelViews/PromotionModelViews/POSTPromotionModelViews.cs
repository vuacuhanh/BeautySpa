namespace BeautySpa.ModelViews.PromotionModelViews
{
    public class POSTPromotionModelView
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? DiscountPercent { get; set; } = 0;
        public decimal? DiscountAmount { get; set; } = 0;
        public int Quantity { get; set; } = 0;
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }

}
