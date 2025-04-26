using BeautySpa.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Rank: BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int MinPoints { get; set; } // Điểm để đạt rank

        public decimal? DiscountPercent { get; set; } // ⭐ Thêm trường này (VD: 10 = 10%)

        public string? Description { get; set; }

        public virtual ICollection<MemberShip> MemberShips { get; set; } = new List<MemberShip>();
        public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
    }
}
