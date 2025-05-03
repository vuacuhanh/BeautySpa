using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BeautySpa.Services.seeding
{
    public static class RankSeeder
    {
        public static async Task SeedRanksAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            var hasAnyRank = await dbContext.Ranks.AnyAsync();
            if (!hasAnyRank)
            {
                var ranks = new List<Rank>
                {
                    new Rank { Id = Guid.NewGuid(), Name = "Bronze", MinPoints = 0, DiscountPercent = 0, Description = "Khách hàng mới", CreatedTime = DateTimeOffset.UtcNow },
                    new Rank { Id = Guid.NewGuid(), Name = "Silver", MinPoints = 5000, DiscountPercent = 2, Description = "Khách hàng thân thiết", CreatedTime = DateTimeOffset.UtcNow },
                    new Rank { Id = Guid.NewGuid(), Name = "Gold", MinPoints = 10000, DiscountPercent = 5, Description = "Khách hàng cao cấp", CreatedTime = DateTimeOffset.UtcNow },
                    new Rank { Id = Guid.NewGuid(), Name = "Platinum", MinPoints = 20000, DiscountPercent = 10, Description = "Khách hàng VIP", CreatedTime = DateTimeOffset.UtcNow }
                };

                await dbContext.Ranks.AddRangeAsync(ranks);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
