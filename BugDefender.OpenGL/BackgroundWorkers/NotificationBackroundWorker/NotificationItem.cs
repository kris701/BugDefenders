using BugDefender.Core.Game.Models;
using static MonoGame.OpenGL.Formatter.Controls.ButtonControl;

namespace BugDefender.OpenGL.BackgroundWorkers.NotificationBackroundWorker
{
    public class NotificationItem
    {
        public string PreFix { get; set; }
        public IDefinition Definition { get; set; }
        public bool HasImage { get; set; }
        public ClickedHandler? Clicked { get; set; }

        public NotificationItem(string preFix, IDefinition definition, bool hasImage)
        {
            PreFix = preFix;
            Definition = definition;
            HasImage = hasImage;
        }

        public NotificationItem(string preFix, IDefinition definition, bool hasImage, ClickedHandler clicked)
        {
            PreFix = preFix;
            Definition = definition;
            HasImage = hasImage;
            Clicked = clicked;
        }
    }
}
