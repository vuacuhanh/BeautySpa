using BeautySpa.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class BranchLocationSpa: BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Description { get; set; } 

        // Mối quan hệ
        public virtual ICollection<LocationSpa> LocationSpas { get; set; } = new List<LocationSpa>(); 
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>(); 
    }
}
