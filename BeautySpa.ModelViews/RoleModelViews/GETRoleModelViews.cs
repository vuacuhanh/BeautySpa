using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.RoleModelViews
{
    public class GETRoleModelViews
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
    }
}
