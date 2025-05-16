using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class SpaBranchLocation : BaseEntity
    {

        public string BranchName { get; set; } = string.Empty;

        public string Street { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = "Vietnam";

        public Guid ProvinceId { get; set; }
        public Guid DistrictId { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Guid ServiceProviderId { get; set; }
        public virtual ServiceProvider? Provider { get; set; }
    }
}
