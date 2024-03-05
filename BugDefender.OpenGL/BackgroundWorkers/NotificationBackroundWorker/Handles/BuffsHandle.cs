using BugDefender.Core.Resources;
using System;
using System.Collections.Generic;

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
            bool any = false;
            foreach (var id in buffs)
            {
                if (_previousBuffs.Contains(id) || Parent.Parent.CurrentUser.Buffs.Contains(id))
                    continue;
                var buff = ResourceManager.Buffs.GetResource(id);
                if (buff.IsValid(Parent.Parent.CurrentUser))
                {
                    _previousBuffs.Add(buff.ID);
                    any = true;
                }
            }
            if (any)
                return new NotificationItem("Buff Available!", new ManualDefinition($"Update Available", $"A new buff is a available. Go to the buff menu to see what it is.."), false, (s) => { Parent.Skip(); });
            return null;
        }
    }
}
