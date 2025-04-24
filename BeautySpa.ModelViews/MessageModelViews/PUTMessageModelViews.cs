namespace BeautySpa.ModelViews.MessageModelViews
{
    public class PUTMessageModelViews
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; }
    }
}
