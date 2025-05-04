namespace BeautySpa.ModelViews.MessageModelViews
{
    public class CreateMessageRequest
    {
        public Guid ReceiverId { get; set; }
        public string ReceiverType { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
