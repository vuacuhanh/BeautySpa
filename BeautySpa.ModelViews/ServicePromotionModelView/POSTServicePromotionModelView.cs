namespace BeautySpa.ModelViews.ServicePromotionModelView
{
    public class POSTServicePromotionModelView
    {
        public Guid ServiceId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public int Quantity { get; set; } = 0;
    }
}