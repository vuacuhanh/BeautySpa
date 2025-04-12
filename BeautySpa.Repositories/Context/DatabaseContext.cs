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
        public DbSet<ServiceProvider> ServiceProviderInfos { get; set; }
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

            // Cấu hình ApplicationUsers
            builder.Entity<ApplicationUsers>()
                .HasOne(u => u.UserInfor)
                .WithOne(ui => ui.User)
                .HasForeignKey<UserInfor>(ui => ui.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUsers>()
                .HasOne(u => u.ServiceProvider)
                .WithOne(spi => spi.Provider)
                .HasForeignKey<ServiceProvider>(spi => spi.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình Service
            builder.Entity<Service>()
                .HasOne(s => s.Provider)
                .WithMany(u => u.Services)
                .HasForeignKey(s => s.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Service>()
                .HasOne(s => s.Category)
                .WithMany(sc => sc.Services)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình WorkingHour
            builder.Entity<WorkingHour>()
                .HasOne(wh => wh.Provider)
                .WithMany(u => u.WorkingHours)
                .HasForeignKey(wh => wh.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WorkingHour>()
                .HasIndex(wh => new { wh.ProviderId, wh.DayOfWeek })
                .IsUnique();

            // Cấu hình Appointment
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
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
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

            // Cấu hình Review
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

            // Cấu hình Favorite
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

            // Cấu hình Notification
            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình Promotion
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

            // Cấu hình ServicePromotion
            builder.Entity<ServicePromotion>()
                .HasOne(sp => sp.Service)
                .WithMany(s => s.ServicePromotions)
                .HasForeignKey(sp => sp.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ServicePromotion>()
                .HasIndex(sp => new { sp.PromotionId, sp.ServiceId })
                .IsUnique();

            // Cấu hình ServiceImage
            builder.Entity<ServiceImage>()
                .HasOne(si => si.Service)
                .WithMany(s => s.ServiceImages)
                .HasForeignKey(si => si.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình Message
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

            // Cấu hình độ chính xác cho các trường decimal
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