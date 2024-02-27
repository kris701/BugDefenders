using BugDefender.OpenGL.Engine.BackgroundWorkers;

namespace BugDefender.OpenGL.BackgroundWorkers.NotificationBackroundWorker
{
    public interface INotificationHandle
    {
        public NotificationBackroundWorker Parent { get; }
        public NotificationItem? GetNewNotification();
    }
}
