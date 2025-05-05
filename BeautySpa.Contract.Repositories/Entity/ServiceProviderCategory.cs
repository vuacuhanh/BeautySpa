using BeautySpa.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class ServiceProviderCategory : BaseEntity
    {
        public Guid ServiceProviderId { get; set; }
        public virtual ServiceProvider? ServiceProvider { get; set; }

        public Guid ServiceCategoryId { get; set; }
        public virtual ServiceCategory? ServiceCategory { get; set; }
    }
}
