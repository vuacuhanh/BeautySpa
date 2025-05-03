namespace BeautySpa.ModelViews.LocationModelViews
{
    public class PUTLocationSpaModelView : POSTLocationSpaModelView
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}
