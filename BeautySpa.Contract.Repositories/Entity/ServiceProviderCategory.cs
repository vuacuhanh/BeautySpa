using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class ServiceProviderCategory : BaseEntity
    {
        public Guid ServiceProviderId { get; set; }
        public virtual ServiceProvider? ServiceProvider { get; set; }

        public Guid ServiceCategoryId { get; set; }
        public virtual ServiceCategory? ServiceCategory { get; set; }
    }
}
