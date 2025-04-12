using BeautySpa.Core.Utils;
using System.ComponentModel.DataAnnotations;


namespace BeautySpa.Core.Base
{
    public abstract class BaseEntity
    {
        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedTime = CoreHelper.SystemTimeNow;
            LastUpdatedTime = CreatedTime;
        }

        [Key]
        public Guid Id { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset? LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
    }
}
