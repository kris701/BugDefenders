﻿using System;
using System.Collections.Generic;
using System.Linq;
using BugDefender.Core.Resources;

namespace BugDefender.OpenGL.BackgroundWorkers.NotificationBackroundWorker.Handles
{
    public class AchivementsHandle : INotificationHandle
    {
        public UIEngine Parent { get; }

        private Guid _currentUserGuid;
        private readonly List<Guid> _previousAchivements = new List<Guid>();

        public AchivementsHandle(UIEngine parent)
        {
            Parent = parent;
            _currentUserGuid = parent.CurrentUser.ID;
            foreach (var achivement in Parent.CurrentUser.Achivements)
                _previousAchivements.Add(achivement);
        }

        public NotificationItem? GetNewNotification()
        {
            if (_currentUserGuid != Parent.CurrentUser.ID)
            {
                _currentUserGuid = Parent.CurrentUser.ID;
                _previousAchivements.Clear();
                foreach (var achivement in Parent.CurrentUser.Achivements)
                    _previousAchivements.Add(achivement);
            }
            var newItem = Parent.CurrentUser.Achivements.FirstOrDefault(x => !_previousAchivements.Contains(x));
            if (newItem != Guid.Empty)
            {
                _previousAchivements.Add(newItem);
                return new NotificationItem("Achivement Unlocked!", ResourceManager.Achivements.GetResource(newItem), true);
            }
            return null;
        }
    }
}
