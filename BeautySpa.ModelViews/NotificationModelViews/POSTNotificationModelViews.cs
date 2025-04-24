namespace BeautySpa.ModelViews.NotificationModelViews
{
    public class POSTNotificationModelViews
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
