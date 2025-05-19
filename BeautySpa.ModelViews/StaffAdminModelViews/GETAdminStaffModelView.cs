
namespace BeautySpa.ModelViews.StaffAdminModelViews
{
    public class GETAdminStaffModelView
    {
        public Guid Id { get; set; }
        public Guid AdminId { get; set; }
        public Guid StaffUserId { get; set; }
        public string StaffRole { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
    }
}
