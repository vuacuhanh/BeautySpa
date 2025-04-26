namespace BeautySpa.ModelViews.AuthModelViews
{
    public class RequestOtpModelView
    {
        public string Email { get; set; }
    }

    public class SignUpWithOtpModelView
    {
        public SignUpAuthModelView SignUp { get; set; }
        public string Otp { get; set; }
    }
}