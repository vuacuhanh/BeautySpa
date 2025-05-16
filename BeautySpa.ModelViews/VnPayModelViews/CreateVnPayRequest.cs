namespace BeautySpa.ModelViews.VnPayModelViews
{
    public class CreateVnPayRequest
    {
        public Guid AppointmentId { get; set; }
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; } = string.Empty;
        public string Locale { get; set; } = "vn";
        public string BankCode { get; set; } = ""; // optional
        public string ReturnUrl { get; set; } = string.Empty;
    }

}
