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
        public virtual ICollection<ServiceProviderCategory>? ServiceProviderCategories { get; set; } = new List<ServiceProviderCategory>(); // Mối quan hệ nhiều-nhiều
        public virtual ICollection<StaffServiceCategory> StaffServiceCategories { get; set; } = new List<StaffServiceCategory>();
    }
}