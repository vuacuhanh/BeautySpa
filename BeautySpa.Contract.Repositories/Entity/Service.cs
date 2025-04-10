using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Service : BaseEntity
    {
        [Required]
        [ForeignKey("ServiceProvider")]
        public string ProviderId { get; set; }

        [Required]
        [ForeignKey("ServiceCategory")]
        public string CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string ServiceName { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Duration { get; set; }

        public decimal? DiscountPrice { get; set; }

        public bool IsAvailable { get; set; } = true;

        public virtual ServiceProvider ServiceProvider { get; set; }
        public virtual ServiceCategory ServiceCategory { get; set; }
    }
}