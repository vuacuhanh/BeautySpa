using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class DepositPolicy : BaseEntity
    {
        public decimal MinPrice { get; set; }          // Ngưỡng tối thiểu
        public decimal MaxPrice { get; set; }          // Ngưỡng tối đa (null = vô hạn)
        public decimal DepositPercent { get; set; }    // % đặt cọc
    }
}