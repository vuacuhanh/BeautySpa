namespace BeautySpa.ModelViews.AuthModelViews
{
    public class TokenResponseModelView
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }

}
