namespace BeautySpa.ModelViews.LocationModelViews
{
    public class PUTBranchLocationModelView : POSTBranchLocationModelView
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}
