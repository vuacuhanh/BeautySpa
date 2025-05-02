namespace BeautySpa.ModelViews.MessageModelViews
{
    public class CreateMessageRequest
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string SenderType { get; set; } = string.Empty;
        public string ReceiverType { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
