using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class ProviderFeePolicy : BaseEntity
    {
        public Guid ProviderId { get; set; }

        // % hệ thống thu khi hoàn thành dịch vụ
        public decimal PlatformFeePercentOnCompleted { get; set; } = 0.05m;

        // % hệ thống thu khi khách hủy/bỏ
        public decimal PlatformFeePercentOnCanceled { get; set; } = 0.02m;

        public virtual ApplicationUsers? Provider { get; set; }
    }
}
