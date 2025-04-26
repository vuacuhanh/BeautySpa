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
        [Required(ErrorMessage = "Branch name is required")]
        [StringLength(100, ErrorMessage = "Branch name must not exceed 100 characters")]
        public string Name { get; set; } 

        [StringLength(20, ErrorMessage = "Phone number must not exceed 20 characters")]
        public string? PhoneNumber { get; set; }

        [StringLength(100, ErrorMessage = "Email must not exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; } 

        [StringLength(500, ErrorMessage = "Description must not exceed 500 characters")]
        public string? Description { get; set; } 

        // Mối quan hệ
        public virtual ICollection<LocationSpa> LocationSpas { get; set; } = new List<LocationSpa>(); 
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>(); 
    }
}
