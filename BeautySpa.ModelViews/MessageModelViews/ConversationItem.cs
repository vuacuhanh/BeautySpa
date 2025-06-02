namespace BeautySpa.ModelViews.MessageModelViews
{
    public class ConversationItem
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = "";
        public string UserAvatar { get; set; } = "";
        public string LastMessage { get; set; } = "";
        public DateTimeOffset LastTime { get; set; }
        public int UnreadCount { get; set; }
    }
}
