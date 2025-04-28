using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Core.Base;

public class RequestBecomeProvider : BaseEntity
{
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string WebsiteOrSocialLink { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Description { get; set; }

    public string RequestStatus { get; set; } = "pending"; // pending, approved, rejected

    public Guid UserId { get; set; }
    public virtual ApplicationUsers? User { get; set; }
}
