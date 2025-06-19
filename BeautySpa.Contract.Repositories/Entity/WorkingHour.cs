using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class WorkingHour : BaseEntity
    {
        public int DayOfWeek { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public bool IsWorking { get; set; } = true;

        public Guid? SpaBranchLocationId { get; set; }
        public virtual SpaBranchLocation? SpaBranchLocation { get; set; }

        public Guid? ProviderId { get; set; }
        public virtual ApplicationUsers? Provider { get; set; }

        public Guid? ServiceProviderId { get; set; }
        public virtual ServiceProvider? ServiceProvider { get; set; }
    }
}