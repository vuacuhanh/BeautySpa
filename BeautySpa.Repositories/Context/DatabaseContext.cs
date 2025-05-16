using BeautySpa.Contract.Repositories.Entity;
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
        public DbSet<ApplicationUserRoles> ApplicationUserRoles => Set<ApplicationUserRoles>();
        public DbSet<ApplicationUserClaim> ApplicationUserClaims => Set<ApplicationUserClaim>();
        public DbSet<ApplicationLogin> ApplicationLogins => Set<ApplicationLogin>();
        public DbSet<ApplicationToken> ApplicationTokens => Set<ApplicationToken>();
        public DbSet<UserInfor> UserInfors => Set<UserInfor>();
        public DbSet<ServiceProvider> ServiceProviders => Set<ServiceProvider>();
        public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<ServiceImage> ServiceImages => Set<ServiceImage>();
        public DbSet<ServicePromotion> ServicePromotions => Set<ServicePromotion>();
        public DbSet<WorkingHour> WorkingHours => Set<WorkingHour>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<AppointmentService> AppointmentServices => Set<AppointmentService>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Favorite> Favorites => Set<Favorite>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<SpaBranchLocation> SpaBranchLocations => Set<SpaBranchLocation>();
        public DbSet<Rank> Ranks => Set<Rank>();
        public DbSet<MemberShip> Memberships => Set<MemberShip>();
        public DbSet<Promotion> Promotions => Set<Promotion>();
        public DbSet<PromotionAdmin> PromotionAdmins => Set<PromotionAdmin>();
        public DbSet<PromotionAdminRank> PromotionAdminRanks => Set<PromotionAdminRank>();
        public DbSet<AdminStaff> AdminStaffs => Set<AdminStaff>();
        public DbSet<Staff> Staffs => Set<Staff>();
        public DbSet<RequestBecomeProvider> RequestBecomeProviders => Set<RequestBecomeProvider>();
        public DbSet<ServiceProviderCategory> ServiceProviderCategories => Set<ServiceProviderCategory>();
        public DbSet<DepositPolicy> DepositPolicys => Set<DepositPolicy>();
        public DbSet<ProviderFeePolicy> ProviderFeePolicys => Set<ProviderFeePolicy>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Appointment relations
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

            // Favorite
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

            // Message
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

            // Review
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

            // Staff
            builder.Entity<Staff>()
                .HasOne(s => s.Provider)
                .WithMany()
                .HasForeignKey(s => s.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Staff>()
                .HasOne(s => s.StaffUser)
                .WithMany()
                .HasForeignKey(s => s.StaffUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // AdminStaff
            builder.Entity<AdminStaff>()
                .HasOne(a => a.Admin)
                .WithMany()
                .HasForeignKey(a => a.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AdminStaff>()
                .HasOne(a => a.StaffUser)
                .WithMany()
                .HasForeignKey(a => a.StaffUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // AppointmentService
            builder.Entity<AppointmentService>()
                .HasOne(x => x.Appointment)
                .WithMany(a => a.AppointmentServices)
                .HasForeignKey(x => x.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AppointmentService>()
                .HasOne(x => x.Service)
                .WithMany()
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // RequestBecomeProvider → User
            builder.Entity<RequestBecomeProvider>()
                .HasOne(r => r.User)
                .WithMany(u => u.BecomeProviderRequests)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ServiceProvider → User
            builder.Entity<ServiceProvider>()
                .HasOne(sp => sp.Provider)
                .WithOne(u => u.ServiceProvider)
                .HasForeignKey<ServiceProvider>(sp => sp.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ServiceImage → ServiceProvider
            builder.Entity<ServiceImage>()
                .HasOne(si => si.ServiceProvider)
                .WithMany(sp => sp.ServiceImages)
                .HasForeignKey(si => si.ServiceProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            // WorkingHour → ServiceProvider
            builder.Entity<WorkingHour>()
                .HasOne(wh => wh.ServiceProvider)
                .WithMany(sp => sp.WorkingHours)
                .HasForeignKey(wh => wh.ServiceProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ServiceProviderCategory many-to-many
            builder.Entity<ServiceProviderCategory>()
                .HasOne(spc => spc.ServiceProvider)
                .WithMany(sp => sp.ServiceProviderCategories)
                .HasForeignKey(spc => spc.ServiceProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ServiceProviderCategory>()
                .HasOne(spc => spc.ServiceCategory)
                .WithMany()
                .HasForeignKey(spc => spc.ServiceCategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            // Precision configs
            builder.Entity<Payment>().Property(p => p.Amount).HasPrecision(10, 2);
            builder.Entity<Payment>().Property(p => p.RefundAmount).HasPrecision(10, 2);
            builder.Entity<Payment>().Property(p => p.PlatformFee).HasPrecision(10, 2);
            builder.Entity<Promotion>().Property(p => p.DiscountAmount).HasPrecision(10, 2);
            builder.Entity<Promotion>().Property(p => p.DiscountPercent).HasPrecision(5, 2);
            builder.Entity<PromotionAdmin>().Property(pa => pa.DiscountAmount).HasPrecision(10, 2);
            builder.Entity<PromotionAdmin>().Property(pa => pa.DiscountPercent).HasPrecision(5, 2);
            builder.Entity<Service>().Property(s => s.Price).HasPrecision(10, 2);
            builder.Entity<ServicePromotion>().Property(sp => sp.DiscountAmount).HasPrecision(10, 2);
            builder.Entity<ServicePromotion>().Property(sp => sp.DiscountPercent).HasPrecision(5, 2);
            builder.Entity<UserInfor>().Property(ui => ui.Salary).HasPrecision(15, 2);
            builder.Entity<Rank>().Property(r => r.DiscountPercent).HasPrecision(5, 2);

            // ServiceImage URL config
            builder.Entity<ServiceImage>()
                .Property(si => si.ImageUrl)
                .IsRequired()
                .HasMaxLength(1000);
        }
    }
}
