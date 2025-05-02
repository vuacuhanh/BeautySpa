using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class ServiceCategory : BaseEntity
    {
        public string CategoryName { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public bool IsActive { get; set; } = true;

        // Mối quan hệ
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    }
}