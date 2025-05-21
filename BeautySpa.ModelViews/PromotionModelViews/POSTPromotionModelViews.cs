namespace BeautySpa.ModelViews.PromotionModelViews
{
    public class POSTPromotionModelView
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public int Quantity { get; set; } = 0;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

}
