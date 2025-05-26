namespace BeautySpa.ModelViews.VnPayModelViews 
{ 
    public class RefundVnPayRequest 
    { 
        public string TransactionId { get; set; } = string.Empty; 
        public decimal Amount { get; set; } 
        public string Reason { get; set; } = string.Empty; 
        public DateTime? TransactionDate { get; set; } 
    } 
}