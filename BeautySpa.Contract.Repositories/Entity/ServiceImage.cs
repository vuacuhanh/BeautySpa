using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Core.Base;

public class ServiceImage : BaseEntity
{
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = false;

    public Guid ServiceProviderId { get; set; }
    public virtual ServiceProvider? ServiceProvider { get; set; }
}
