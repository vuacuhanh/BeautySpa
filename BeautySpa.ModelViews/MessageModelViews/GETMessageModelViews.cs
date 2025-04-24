namespace BeautySpa.ModelViews.MessageModelViews
{
    public class GETMessageModelViews
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public string SenderType { get; set; } = string.Empty;
        public string ReceiverType { get; set; } = string.Empty;

        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }

        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset? LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
    }
}
