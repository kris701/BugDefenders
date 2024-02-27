namespace BugDefender.OpenGL.BackgroundWorkers.NotificationBackroundWorker
{
    public interface INotificationHandle
    {
        public UIEngine Parent { get; }
        public NotificationItem? GetNewNotification();
    }
}
