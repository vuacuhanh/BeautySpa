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
        [Required(ErrorMessage = "Street is required")]
        [StringLength(200, ErrorMessage = "Street must not exceed 200 characters")]
        public string Street { get; set; } // Đường: "123 Đường Láng"

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, ErrorMessage = "City must not exceed 100 characters")]
        public string City { get; set; } // Thành phố: "Hà Nội"

        [StringLength(20, ErrorMessage = "Postal code must not exceed 20 characters")]
        public string? PostalCode { get; set; } // Mã bưu điện: "100000"

        [StringLength(100, ErrorMessage = "Country must not exceed 100 characters")]
        public string? Country { get; set; } // Quốc gia: "Vietnam"

        [StringLength(500, ErrorMessage = "Description must not exceed 500 characters")]
        public string? Description { get; set; } // Mô tả: "Gần ngã tư Đường Láng"

        // Khóa ngoại
        public Guid BranchId { get; set; } // Liên kết với chi nhánh
        public virtual BranchLocationSpa Branch { get; set; } // Chi nhánh sở hữu địa chỉ này

        // Mối quan hệ
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>(); 

    }
}
