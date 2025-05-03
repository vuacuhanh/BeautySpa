namespace BeautySpa.ModelViews.PromotionModelViews
{
    public class GETPromotionModelViews : PUTPromotionModelViews
    {
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset? LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
    }
}


