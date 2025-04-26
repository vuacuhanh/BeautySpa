using BeautySpa.Core.Utils;
using Microsoft.AspNetCore.Identity;


namespace BeautySpa.Contract.Repositories.Entity
{
    public class ApplicationUsers : IdentityUser<Guid>
    {
        public bool IsVerified { get; set; } = false;
        public string Status { get; set; } = "active";
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }

        // Mối quan hệ
        public virtual MemberShip MemberShip { get; set; } = default!;
        public virtual UserInfor? UserInfor { get; set; }
        public virtual ServiceProvider? ServiceProvider { get; set; }
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
        public virtual ICollection<WorkingHour> WorkingHours { get; set; } = new List<WorkingHour>();
        public virtual ICollection<Appointment> CustomerAppointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Appointment> ProviderAppointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Review> CustomerReviews { get; set; } = new List<Review>();
        public virtual ICollection<Review> ProviderReviews { get; set; } = new List<Review>();
        public virtual ICollection<Favorite> CustomerFavorites { get; set; } = new List<Favorite>();
        public virtual ICollection<Favorite> ProviderFavorites { get; set; } = new List<Favorite>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
        public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

        public ApplicationUsers()
        {
            CreatedTime = DateTimeOffset.UtcNow;
            LastUpdatedTime = CreatedTime;
        }
    }
}
