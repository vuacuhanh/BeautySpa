using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Promotion : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Chỉ dùng 1 loại giảm giá: phần trăm hoặc số tiền
        public decimal? DiscountPercent { get; set; }   // Giảm theo %
        public decimal? DiscountAmount { get; set; }    // Giảm theo số tiền
        public int Quantity { get; set; } = 0;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Nhà cung cấp tạo khuyến mãi
        public Guid ProviderId { get; set; }
        public virtual ApplicationUsers? Provider { get; set; }

        // Danh sách lịch hẹn đã áp dụng khuyến mãi
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
