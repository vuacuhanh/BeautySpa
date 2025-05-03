namespace BeautySpa.ModelViews.MemberShipModelViews
{
    public class GETMemberShipModelView
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? FullName { get; set; }
        public int AccumulatedPoints { get; set; }
        public string? RankName { get; set; }
        public DateTimeOffset? LastRankUpdate { get; set; }
    }
}
