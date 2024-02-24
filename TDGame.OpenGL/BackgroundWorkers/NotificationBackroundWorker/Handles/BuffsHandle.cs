using System;
using System.Collections.Generic;
using TDGame.Core.Resources;

namespace TDGame.OpenGL.BackgroundWorkers.NotificationBackroundWorker.Handles
{
    public class BuffsHandle : INotificationHandle
    {
        public UIEngine Parent { get; }

        private Guid _currentUserGuid;
        private readonly List<Guid> _previousBuffs = new List<Guid>();

        public BuffsHandle(UIEngine parent)
        {
            Parent = parent;
            _currentUserGuid = parent.CurrentUser.ID;
            foreach (var achivement in Parent.CurrentUser.Buffs)
                _previousBuffs.Add(achivement);
        }

        public NotificationItem? GetNewNotification()
        {
            if (_currentUserGuid != Parent.CurrentUser.ID)
            {
                _currentUserGuid = Parent.CurrentUser.ID;
                _previousBuffs.Clear();
                foreach (var achivement in Parent.CurrentUser.Buffs)
                    _previousBuffs.Add(achivement);
            }
            var buffs = ResourceManager.Buffs.GetResources();
            foreach (var id in buffs)
            {
                if (_previousBuffs.Contains(id) || Parent.CurrentUser.Buffs.Contains(id))
                    continue;
                var buff = ResourceManager.Buffs.GetResource(id);
                if (buff.IsValid(Parent.CurrentUser))
                {
                    _previousBuffs.Add(buff.ID);
                    return new NotificationItem("Buff Available!", buff, false);
                }
            }
            return null;
        }
    }
}
