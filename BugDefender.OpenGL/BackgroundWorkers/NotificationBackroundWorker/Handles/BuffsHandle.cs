using System;
using System.Collections.Generic;
using BugDefender.Core.Resources;
using BugDefender.OpenGL.Engine.BackgroundWorkers;

namespace BugDefender.OpenGL.BackgroundWorkers.NotificationBackroundWorker.Handles
{
    public class BuffsHandle : INotificationHandle
    {
        public NotificationBackroundWorker Parent { get; }

        private Guid _currentUserGuid;
        private readonly List<Guid> _previousBuffs = new List<Guid>();

        public BuffsHandle(NotificationBackroundWorker parent)
        {
            Parent = parent;
            _currentUserGuid = parent.Parent.CurrentUser.ID;
            foreach (var achivement in Parent.Parent.CurrentUser.Buffs)
                _previousBuffs.Add(achivement);
        }

        public NotificationItem? GetNewNotification()
        {
            if (_currentUserGuid != Parent.Parent.CurrentUser.ID)
            {
                _currentUserGuid = Parent.Parent.CurrentUser.ID;
                _previousBuffs.Clear();
                foreach (var achivement in Parent.Parent.CurrentUser.Buffs)
                    _previousBuffs.Add(achivement);
            }
            var buffs = ResourceManager.Buffs.GetResources();
            foreach (var id in buffs)
            {
                if (_previousBuffs.Contains(id) || Parent.Parent.CurrentUser.Buffs.Contains(id))
                    continue;
                var buff = ResourceManager.Buffs.GetResource(id);
                if (buff.IsValid(Parent.Parent.CurrentUser))
                {
                    _previousBuffs.Add(buff.ID);
                    return new NotificationItem("Buff Available!", buff, false, (s) => { Parent.Skip(); });
                }
            }
            return null;
        }
    }
}
