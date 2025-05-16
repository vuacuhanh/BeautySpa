namespace BeautySpa.ModelViews.LocationModelViews
{
    public class GETSpaBranchLocationModelView
    {
        public Guid Id { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}   