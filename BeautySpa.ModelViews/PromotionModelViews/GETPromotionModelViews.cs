namespace BeautySpa.ModelViews.PromotionModelViews
{
    public class GETPromotionModelView
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public int Quantity { get; set; } = 0;
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public bool IsActive { get; set; }
        public Guid ProviderId { get; set; }
        public string? ProviderName { get; set; }
    }
}


