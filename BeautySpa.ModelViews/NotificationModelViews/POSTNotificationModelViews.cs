namespace BeautySpa.ModelViews.NotificationModelViews
{
    public class POSTNotificationModelView
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
    }
}
