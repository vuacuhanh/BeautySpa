using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BeautySpa.Contract.Repositories.Entity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BeautySpa.Repositories.Context
{
    public class DatabaseContext : IdentityDbContext<ApplicationUsers, ApplicationRoles, Guid>
    {
        public DatabaseContext(DbContextOptions <DatabaseContext> options):base(options) { }

        public virtual DbSet<ApplicationUsers> ApplicationUsers => Set<ApplicationUsers>();
        public virtual DbSet<ApplicationRoles> ApplicationRoles => Set<ApplicationRoles>();
        public virtual DbSet<ApplicationRoleClaims> ApplicationRoleClaims => Set<ApplicationRoleClaims>();
        public virtual DbSet<ApplicationLogin> ApplicationLogins => Set<ApplicationLogin>();
        public virtual DbSet<ApplicationToken> ApplicationTokens => Set<ApplicationToken>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
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
        }

    }
}
