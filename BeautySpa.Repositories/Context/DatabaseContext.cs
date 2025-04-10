using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BeautySpa.Contract.Repositories.Entity;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Repositories.Context
{
    public class DatabaseContext : IdentityDbContext<ApplicationUsers, ApplicationRoles, Guid>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public virtual DbSet<ApplicationUsers> ApplicationUsers => Set<ApplicationUsers>();
        public virtual DbSet<ApplicationRoles> ApplicationRoles => Set<ApplicationRoles>();
        public virtual DbSet<ApplicationRoleClaims> ApplicationRoleClaims => Set<ApplicationRoleClaims>();
        public virtual DbSet<ApplicationLogin> ApplicationLogins => Set<ApplicationLogin>();
        public virtual DbSet<ApplicationToken> ApplicationTokens => Set<ApplicationToken>();
        public virtual DbSet<UserInfor> UserInfors => Set<UserInfor>();


        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<ServiceProvider> ServiceProviders { get; set; }
        public virtual DbSet<ServiceCategory> ServiceCategories { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<WorkingHour> WorkingHours { get; set; }
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Favorite> Favorites { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<ServicePromotion> ServicePromotions { get; set; }
        public virtual DbSet<ServiceImage> ServiceImages { get; set; }
        public virtual DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Identity relationships
            builder.Entity<ApplicationUserRoles>()
                .HasOne<ApplicationUsers>()
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUserRoles>()
                .HasOne<ApplicationRoles>()
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure unique constraints
            builder.Entity<Customers>()
                .HasIndex(c => c.PhoneNumber)
                .IsUnique();

            builder.Entity<ServiceProvider>()
                .HasIndex(sp => sp.PhoneNumber)
                .IsUnique();

            builder.Entity<WorkingHour>()
                .HasIndex(wh => new { wh.ProviderId, wh.DayOfWeek })
                .IsUnique();

            builder.Entity<Favorite>()
                .HasIndex(f => new { f.CustomerId, f.ProviderId })
                .IsUnique();

            // ServicePromotion with safe delete behavior
            builder.Entity<ServicePromotion>()
                .HasIndex(sp => new { sp.PromotionId, sp.ServiceId })
                .IsUnique();

            builder.Entity<ServicePromotion>()
                .HasOne(sp => sp.Promotion)
                .WithMany()
                .HasForeignKey(sp => sp.PromotionId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ServicePromotion>()
                .HasOne(sp => sp.Service)
                .WithMany()
                .HasForeignKey(sp => sp.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);

            // Appointment relationships
            builder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany()
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany()
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Appointment>()
                .HasOne(a => a.ServiceProvider)
                .WithMany()
                .HasForeignKey(a => a.ProviderId)
                .OnDelete(DeleteBehavior.NoAction);

            // Service relationships
            builder.Entity<Service>()
                .HasOne(s => s.ServiceProvider)
                .WithMany()
                .HasForeignKey(s => s.ProviderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Service>()
                .HasOne(s => s.ServiceCategory)
                .WithMany()
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            // Review relationships
            builder.Entity<Review>()
                .HasOne(r => r.Appointment)
                .WithMany()
                .HasForeignKey(r => r.AppointmentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Review>()
                .HasOne(r => r.Customer)
                .WithMany()
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Review>()
                .HasOne(r => r.ServiceProvider)
                .WithMany()
                .HasForeignKey(r => r.ProviderId)
                .OnDelete(DeleteBehavior.NoAction);

            // Payment relationship
            builder.Entity<Payment>()
                .HasOne(p => p.Appointment)
                .WithMany()
                .HasForeignKey(p => p.AppointmentId)
                .OnDelete(DeleteBehavior.NoAction);

            // ServiceImage relationship
            builder.Entity<ServiceImage>()
                .HasOne(si => si.Service)
                .WithMany()
                .HasForeignKey(si => si.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);

            // WorkingHour relationship
            builder.Entity<WorkingHour>()
                .HasOne(wh => wh.ServiceProvider)
                .WithMany()
                .HasForeignKey(wh => wh.ProviderId)
                .OnDelete(DeleteBehavior.NoAction);

            // Promotion relationship
            builder.Entity<Promotion>()
                .HasOne(p => p.ServiceProvider)
                .WithMany()
                .HasForeignKey(p => p.ProviderId)
                .OnDelete(DeleteBehavior.NoAction);

            // Add indexes for performance
            builder.Entity<Appointment>()
                .HasIndex(a => new { a.AppointmentDate, a.Status });

            builder.Entity<Service>()
                .HasIndex(s => s.IsAvailable);

            builder.Entity<ServiceProvider>()
                .HasIndex(sp => sp.IsApproved);

            builder.Entity<Notification>()
                .HasIndex(n => new { n.UserId, n.IsRead });
        }
    }
}