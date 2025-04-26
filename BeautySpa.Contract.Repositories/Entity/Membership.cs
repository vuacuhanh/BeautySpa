using BeautySpa.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class MemberShip : BaseEntity
    {
        public Guid UserId { get; set; }
        public virtual ApplicationUsers User { get; set; }

        public int AccumulatedPoints { get; set; }

        public Guid RankId { get; set; }
        public virtual Rank Rank { get; set; }
    }
}
