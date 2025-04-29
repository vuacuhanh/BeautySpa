using BeautySpa.Contract.Repositories.Entity;
using Microsoft.AspNetCore.Identity;

namespace BeautySpa.Services.seeding
{
    public static class RoleSeeder
    {
        private static readonly List<string> DefaultRoles = new()
        {
            "Admin",
            "Customer",
            "Provider"
        };

        public static async Task SeedRolesAsync(RoleManager<ApplicationRoles> roleManager)
        {
            foreach (var roleName in DefaultRoles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new ApplicationRoles
                    {
                        Id = Guid.NewGuid(),
                        Name = roleName,
                        NormalizedName = roleName.ToUpper(),
                        CreatedTime = DateTime.UtcNow,
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    };

                    await roleManager.CreateAsync(role);
                }
            }
        }
    }
}
