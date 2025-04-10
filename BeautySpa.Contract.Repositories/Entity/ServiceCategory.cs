using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class ServiceCategory : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; }

        [StringLength(255)]
        public string IconUrl { get; set; }

        public bool IsActive { get; set; } = true;
    }
}