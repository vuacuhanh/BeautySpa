using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class WorkingHour : BaseEntity
    {
        [Required]
        [ForeignKey("ServiceProvider")]
        public string ProviderId { get; set; }

        [Required]
        public byte DayOfWeek { get; set; }

        [Required]
        public TimeSpan OpeningTime { get; set; }

        [Required]
        public TimeSpan ClosingTime { get; set; }

        public bool IsWorking { get; set; } = true;

        public virtual ServiceProvider ServiceProvider { get; set; }
    }
}