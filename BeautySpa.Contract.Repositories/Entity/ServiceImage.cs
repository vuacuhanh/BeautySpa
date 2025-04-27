using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class ServiceImage : BaseEntity
    {
        public string ImageUrl { get; set; } = string.Empty;
<<<<<<< HEAD
        public bool IsPrimary { get; set; } = false; 
=======
        public bool IsPrimary { get; set; } = false; // true = ảnh chính, false = ảnh phụ
>>>>>>> 73ac29296ce57368183dd2037897957d584a79d1

        // Khóa ngoại
        public Guid ServiceProviderId { get; set; }
        public virtual ServiceProvider ServiceProvider { get; set; }
    }
}