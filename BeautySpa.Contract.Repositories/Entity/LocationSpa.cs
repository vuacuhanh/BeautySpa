using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class LocationSpa : BaseEntity
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Map & Tỉnh/Thành
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }

        // Trạng thái hoạt động địa điểm
        public bool IsActive { get; set; } = true;

        // FK
        public Guid BranchId { get; set; }
        public virtual BranchLocationSpa? Branch { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
