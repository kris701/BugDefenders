using BugDefender.Core.Game.Models;

namespace BugDefender.OpenGL.BackgroundWorkers.NotificationBackroundWorker
{
    public class NotificationItem
    {
        public string PreFix { get; set; }
        public IDefinition Definition { get; set; }
        public bool HasImage { get; set; }

        public NotificationItem(string preFix, IDefinition definition, bool hasImage)
        {
            PreFix = preFix;
            Definition = definition;
            HasImage = hasImage;
        }
    }
}
