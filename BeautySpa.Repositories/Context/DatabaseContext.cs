using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Core.Base;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Repositories.Context
{
    public class DatabaseContext : IdentityDbContext<ApplicationUsers, ApplicationRoles, Guid>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        // DbSets (Viết đồng bộ => Set<T>())
        public DbSet<ApplicationUsers> ApplicationUsers => Set<ApplicationUsers>();
        public DbSet<ApplicationRoles> ApplicationRoles => Set<ApplicationRoles>();
        public DbSet<ApplicationRoleClaims> ApplicationRoleClaims => Set<ApplicationRoleClaims>();
        public DbSet<ApplicationLogin> ApplicationLogins => Set<ApplicationLogin>();
        public DbSet<ApplicationToken> ApplicationTokens => Set<ApplicationToken>();
        public DbSet<UserInfor> UserInfors => Set<UserInfor>();
        public DbSet<ServiceProvider> ServiceProviderInfos => Set<ServiceProvider>();
        public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<WorkingHour> WorkingHours => Set<WorkingHour>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Favorite> Favorites => Set<Favorite>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Promotion> Promotions => Set<Promotion>();
        public DbSet<ServicePromotion> ServicePromotions => Set<ServicePromotion>();
        public DbSet<ServiceImage> ServiceImages => Set<ServiceImage>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<LocationSpa> LocationSpa => Set<LocationSpa>();
        public DbSet<BranchLocationSpa> BranchLocationSpas => Set<BranchLocationSpa>();
        public DbSet<Rank> Ranks => Set<Rank>();
        public DbSet<MemberShip> Memberships => Set<MemberShip>();
        public DbSet<PromotionAdmin> PromotionAdmins => Set<PromotionAdmin>();

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

<<<<<<< HEAD
            // Cấu hình PromotionAdmin
            builder.Entity<PromotionAdmin>()
                .HasOne(pa => pa.Rank)
                .WithMany()
                .HasForeignKey(pa => pa.RankId)
                .OnDelete(DeleteBehavior.SetNull);
=======
            // Cấu hình ServicePromotion
            builder.Entity<ServicePromotion>()
                .HasOne(sp => sp.Service)
                .WithMany(s => s.ServicePromotions)
                .HasForeignKey(sp => sp.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ServicePromotion>()
                .HasIndex(sp => new { sp.PromotionId, sp.ServiceId })
                .IsUnique();


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

            // Cấu hình BranchLocationSpa & LocationSpa
            builder.Entity<LocationSpa>()
                .HasOne(ls => ls.Branch)
                .WithMany(bl => bl.LocationSpas)
                .HasForeignKey(ls => ls.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BranchLocationSpa>()
                .HasMany(bl => bl.LocationSpas)
                .WithOne(ls => ls.Branch)
                .HasForeignKey(ls => ls.BranchId);

            builder.Entity<Appointment>()
            .HasOne(a => a.LocationSpa)
            .WithMany(ls => ls.Appointments)
            .HasForeignKey(a => a.LocationSpaId)
            .OnDelete(DeleteBehavior.Restrict);

            //
            builder.Entity<MemberShip>()
            .HasOne(ms => ms.User)
            .WithOne(u => u.MemberShip)
            .HasForeignKey<MemberShip>(ms => ms.UserId);

            builder.Entity<MemberShip>()
                .HasOne(ms => ms.Rank)
                .WithMany(r => r.MemberShips)
                .HasForeignKey(ms => ms.RankId);

            builder.Entity<Promotion>()
                .HasOne(p => p.RequiredRank)
                .WithMany(r => r.Promotions)
                .HasForeignKey(p => p.RequiredRankId)
                .OnDelete(DeleteBehavior.Restrict);
>>>>>>> 73ac29296ce57368183dd2037897957d584a79d1


            // ==== Precision các trường decimal ====
            builder.Entity<Payment>()
                 .Property(p => p.Amount)
                 .HasPrecision(10, 2);

            builder.Entity<Promotion>()
                .Property(p => p.DiscountAmount)
                .HasPrecision(10, 2);

            builder.Entity<Promotion>()
                .Property(p => p.DiscountPercent)
                .HasPrecision(5, 2);

            builder.Entity<PromotionAdmin>()
                .Property(pa => pa.DiscountAmount)
                .HasPrecision(10, 2);

            builder.Entity<PromotionAdmin>()
                .Property(pa => pa.DiscountPercent)
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
