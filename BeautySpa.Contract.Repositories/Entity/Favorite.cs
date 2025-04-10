using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Favorite : BaseEntity
    {
        [Required]
        [ForeignKey("Customer")]
        public string CustomerId { get; set; }

        [Required]
        [ForeignKey("ServiceProvider")]
        public string ProviderId { get; set; }

        public virtual Customers Customer { get; set; }
        public virtual ServiceProvider ServiceProvider { get; set; }
    }
}