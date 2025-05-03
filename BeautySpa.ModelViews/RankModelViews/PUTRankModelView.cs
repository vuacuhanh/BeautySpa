namespace BeautySpa.ModelViews.RankModelViews
{
    public class PUTRankModelView
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MinPoints { get; set; }
        public decimal? DiscountPercent { get; set; }
        public string? Description { get; set; }
    }
}
