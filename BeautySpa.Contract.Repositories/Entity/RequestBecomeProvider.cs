using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class RequestBecomeProvider : BaseEntity
    {
        public string BusinessName { get; set; } = string.Empty;
        public string BusinessType { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string WebsiteOrSocialLink { get; set; } = string.Empty;
        public string? Address { get; set; }

        public string? Description { get; set; }

        public string RequestStatus { get; set; } = "pending";

        public Guid UserId { get; set; }
        public virtual ApplicationUsers? User { get; set; }
    }
}