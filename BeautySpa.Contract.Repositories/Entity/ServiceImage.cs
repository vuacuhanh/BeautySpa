using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class ServiceImage : BaseEntity
    {
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false; 

        // Khóa ngoại
        public Guid ServiceProviderId { get; set; }
        public virtual ServiceProvider ServiceProvider { get; set; }
    }
}