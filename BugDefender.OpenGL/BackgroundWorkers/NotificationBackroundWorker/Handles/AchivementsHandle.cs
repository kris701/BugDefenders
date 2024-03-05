using BugDefender.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BugDefender.OpenGL.BackgroundWorkers.NotificationBackroundWorker.Handles
{
    public class AchivementsHandle : INotificationHandle
    {
        public NotificationBackroundWorker Parent { get; }

        private Guid _currentUserGuid;
        private readonly List<Guid> _previousAchivements = new List<Guid>();

        public AchivementsHandle(NotificationBackroundWorker parent)
        {
            Parent = parent;
            _currentUserGuid = parent.Parent.UserManager.CurrentUser.ID;
            foreach (var achivement in Parent.Parent.UserManager.CurrentUser.Achivements)
                _previousAchivements.Add(achivement);
        }

        public NotificationItem? GetNewNotification()
        {
            if (_currentUserGuid != Parent.Parent.UserManager.CurrentUser.ID)
            {
                _currentUserGuid = Parent.Parent.UserManager.CurrentUser.ID;
                _previousAchivements.Clear();
                foreach (var achivement in Parent.Parent.UserManager.CurrentUser.Achivements)
                    _previousAchivements.Add(achivement);
            }
            var newItem = Parent.Parent.UserManager.CurrentUser.Achivements.FirstOrDefault(x => !_previousAchivements.Contains(x));
            if (newItem != Guid.Empty)
            {
                _previousAchivements.Add(newItem);
                return new NotificationItem("Achivement Unlocked!", ResourceManager.Achivements.GetResource(newItem), true, (s) => { Parent.Skip(); });
            }
            return null;
        }
    }
}
