using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class WorkingHour : BaseEntity
    {
        public int DayOfWeek { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public bool IsWorking { get; set; } = true;

        // Khóa ngoại
        public Guid ProviderId { get; set; }
        public virtual ApplicationUsers Provider { get; set; }
    }
}