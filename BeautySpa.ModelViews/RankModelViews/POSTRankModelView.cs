namespace BeautySpa.ModelViews.RankModelViews
{
    public class POSTRankModelView
    {
        public string Name { get; set; } = string.Empty;
        public int MinPoints { get; set; }
        public decimal? DiscountPercent { get; set; }
        public string? Description { get; set; }
    }
}
