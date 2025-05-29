using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class StaffServiceCategory: BaseEntity
    {
        public Guid StaffId { get; set; }
        public Staff? Staff { get; set; }

        public Guid ServiceCategoryId { get; set; }
        public ServiceCategory? ServiceCategory { get; set; }
    }
}
