namespace BeautySpa.ModelViews.PromotionAdminModelView
{
    public class GETPromotionAdminModelView
    {
        public Guid Id { get; set; }
        public string PromotionName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        public List<Guid> RankIds { get; set; } = new();
    }

}
