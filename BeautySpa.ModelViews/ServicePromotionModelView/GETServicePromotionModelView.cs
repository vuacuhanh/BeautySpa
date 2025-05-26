namespace BeautySpa.ModelViews.ServicePromotionModelView
{
    public class GETServicePromotionModelView
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public int Quantity { get; set; } = 0;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string? ServiceName { get; set; }
    }
}