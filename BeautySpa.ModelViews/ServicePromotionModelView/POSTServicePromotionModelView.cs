namespace BeautySpa.ModelViews.ServicePromotionModelView
{
    public class POSTServicePromotionModelView
    {
        public Guid ServiceId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public int Quantity { get; set; } = 0;
    }
}