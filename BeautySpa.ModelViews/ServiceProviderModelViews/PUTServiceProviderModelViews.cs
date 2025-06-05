using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.ServiceProviderModelViews
{
    public class PUTServiceProviderModelViews
    {
        public Guid Id { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ContactFullName { get; set; } = string.Empty;
        public string ContactPosition { get; set; } = string.Empty;
        public int? YearsOfExperience { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public bool IsApproved { get; set; }
        public string Status { get; set; } = "pending";
        public string? AddressDetail { get; set; }
        public string? ProvinceId { get; set; }
        public string? DistrictId { get; set; }
        public string? ProvinceName { get; set; }
        public string? DistrictName { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
<<<<<<< HEAD
        public int MaxAppointmentsPerSlot { get; set; } = 5;
=======
        public int MaxAppointmentsPerSlot { get; set; } = 0;
>>>>>>> 69164a2c523de3343cce845b080745ed3f4d99bd
        public string? Email { get; set; }
        //public Guid ProviderId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public List<Guid>? ServiceCategoryIds { get; set; }
    }
}