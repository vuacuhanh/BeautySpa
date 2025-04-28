using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Favorite : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public virtual ApplicationUsers? Customer { get; set; }

        public Guid ProviderId { get; set; }
        public virtual ApplicationUsers? Provider { get; set; }
    }
}