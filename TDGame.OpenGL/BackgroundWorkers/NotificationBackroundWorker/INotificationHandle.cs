using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Game.Models;

namespace TDGame.OpenGL.BackgroundWorkers.NotificationBackroundWorker
{
    public interface INotificationHandle
    {
        public UIEngine Parent { get; }
        public NotificationItem? GetNewNotification(); 
    }
}
