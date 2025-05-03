
namespace BeautySpa.ModelViews.StaffAdminModelViews
{
    public class PUTAdminStaffModelView
    {
        public Guid Id { get; set; }
        public string StaffRole { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
    }
}
