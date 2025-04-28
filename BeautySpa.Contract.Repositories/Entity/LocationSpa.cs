using BeautySpa.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class LocationSpa: BaseEntity
    {
        public string Street { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string PostalCode { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Khóa ngoại
        public Guid BranchId { get; set; }
        public virtual BranchLocationSpa? Branch { get; set; }

        // Mối quan hệ
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>(); 

    }
}
