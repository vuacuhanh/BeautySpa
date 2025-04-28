namespace BeautySpa.ModelViews.AuthModelViews
{
    public class ResetPasswordAuthModelView
    {
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

}
