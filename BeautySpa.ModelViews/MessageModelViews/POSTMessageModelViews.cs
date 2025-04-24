namespace BeautySpa.ModelViews.MessageModelViews
{
    public class POSTMessageModelViews
    {
        public string Content { get; set; } = string.Empty;
        public string SenderType { get; set; } = string.Empty;
        public string ReceiverType { get; set; } = string.Empty;

        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
    }
}
