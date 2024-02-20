using TDGame.Core.Game.Models;

namespace TDGame.OpenGL.BackgroundWorkers.NotificationBackroundWorker
{
    public class NotificationItem
    {
        public string PreFix { get; set; }
        public IDefinition Definition { get; set; }
        public bool HasImage { get; set; }
    }
}
