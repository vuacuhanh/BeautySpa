namespace BeautySpa.ModelViews.PromotionModelViews
{
    public class PUTPromotionModelViews : POSTPromotionModelViews
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}
