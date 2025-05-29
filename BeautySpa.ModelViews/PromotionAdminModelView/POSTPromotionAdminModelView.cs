namespace BeautySpa.ModelViews.PromotionAdminModelView
{
    public class POSTPromotionAdminModelView
    {
        public string PromotionName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercent { get; set; }
        public int Quantity { get; set; } = 0;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Guid> RankIds { get; set; } = new();
    }

}
