using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class ServiceImage : BaseEntity
    {
        public string ImageUrl { get; set; } = string.Empty;

        public Guid ServiceProviderId { get; set; }
        public virtual ServiceProvider? ServiceProvider { get; set; }
    }
}