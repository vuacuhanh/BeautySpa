namespace BeautySpa.ModelViews.NotificationModelViews
{
    public class GETNotificationModelView
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
    }
}
