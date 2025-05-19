using BeautySpa.Core.Base;
using BeautySpa.Core.Utils;
using System.ComponentModel.DataAnnotations;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class MemberShip : BaseEntity
    {
        public Guid UserId { get; set; }
        [Range(0, int.MaxValue)]
        public int AccumulatedPoints { get; set; } = 0;
        public Guid RankId { get; set; }
        public virtual Rank? Rank { get; set; }
        public virtual ApplicationUsers? User { get; set; }
        public DateTimeOffset? LastRankUpdate { get; set; } = CoreHelper.SystemTimeNow;
    }
}