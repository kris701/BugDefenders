using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
