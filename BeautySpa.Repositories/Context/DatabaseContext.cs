using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Core.Base;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Repositories.Context
{
    public class DatabaseContext : IdentityDbContext<ApplicationUsers, ApplicationRoles, Guid>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        // DbSets
        public DbSet<ApplicationUsers> ApplicationUsers => Set<ApplicationUsers>();
        public DbSet<ApplicationRoles> ApplicationRoles => Set<ApplicationRoles>();
        public DbSet<ApplicationRoleClaims> ApplicationRoleClaims => Set<ApplicationRoleClaims>();
        public DbSet<ApplicationLogin> ApplicationLogins => Set<ApplicationLogin>();
        public DbSet<ApplicationToken> ApplicationTokens => Set<ApplicationToken>();
        public DbSet<UserInfor> UserInfors { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<WorkingHour> WorkingHours { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<ServicePromotion> ServicePromotions { get; set; }
        public DbSet<ServiceImage> ServiceImages { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure ApplicationUsers
            builder.Entity<ApplicationUsers>()
                .HasOne(u => u.UserInfor)
                .WithOne(ui => ui.User)
                .HasForeignKey<UserInfor>(ui => ui.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ServiceCategory
            builder.Entity<ServiceCategory>()
                .HasMany(sc => sc.Services)
                .WithOne(s => s.Category)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Service
            builder.Entity<Service>()
                .HasOne(s => s.Provider)
                .WithMany(u => u.Services)
                .HasForeignKey(s => s.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Service>()
                .HasMany(s => s.Appointments)
                .WithOne(a => a.Service)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Service>()
                .HasMany(s => s.ServicePromotions)
                .WithOne(sp => sp.Service)
                .HasForeignKey(sp => sp.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Service>()
                .HasMany(s => s.ServiceImages)
                .WithOne(si => si.Service)
                .HasForeignKey(si => si.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure WorkingHour
            builder.Entity<WorkingHour>()
                .HasOne(wh => wh.Provider)
                .WithMany(u => u.WorkingHours)
                .HasForeignKey(wh => wh.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WorkingHour>()
                .HasIndex(wh => new { wh.ProviderId, wh.DayOfWeek })
                .IsUnique();

            // Configure Appointment
            builder.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany(u => u.CustomerAppointments)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Provider)
                .WithMany(u => u.ProviderAppointments)
                .HasForeignKey(a => a.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Payment)
                .WithOne(p => p.Appointment)
                .HasForeignKey<Payment>(p => p.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Appointment>()
                .HasOne(a => a.Review)
                .WithOne(r => r.Appointment)
                .HasForeignKey<Review>(r => r.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Review
            builder.Entity<Review>()
                .HasOne(r => r.Customer)
                .WithMany(u => u.CustomerReviews)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.Provider)
                .WithMany(u => u.ProviderReviews)
                .HasForeignKey(r => r.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Favorite
            builder.Entity<Favorite>()
                .HasOne(f => f.Customer)
                .WithMany(u => u.CustomerFavorites)
                .HasForeignKey(f => f.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Favorite>()
                .HasOne(f => f.Provider)
                .WithMany(u => u.ProviderFavorites)
                .HasForeignKey(f => f.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Favorite>()
                .HasIndex(f => new { f.CustomerId, f.ProviderId })
                .IsUnique();

            // Configure Notification
            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Promotion
            builder.Entity<Promotion>()
                .HasOne(p => p.Provider)
                .WithMany(u => u.Promotions)
                .HasForeignKey(p => p.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Promotion>()
                .HasMany(p => p.ServicePromotions)
                .WithOne(sp => sp.Promotion)
                .HasForeignKey(sp => sp.PromotionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ServicePromotion
            builder.Entity<ServicePromotion>()
                .HasIndex(sp => new { sp.PromotionId, sp.ServiceId })
                .IsUnique();

            // Configure Message
            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure decimal precision
            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(10, 2);

            builder.Entity<Promotion>()
                .Property(p => p.DiscountAmount)
                .HasPrecision(10, 2);

            builder.Entity<Promotion>()
                .Property(p => p.DiscountPercent)
                .HasPrecision(5, 2);

            builder.Entity<Service>()
                .Property(s => s.Price)
                .HasPrecision(10, 2);

            builder.Entity<Service>()
                .Property(s => s.DiscountPrice)
                .HasPrecision(10, 2);

            builder.Entity<UserInfor>()
                .Property(ui => ui.Salary)
                .HasPrecision(15, 2);
        }
    }
}