using BugDefender.Core.Game.Models;
using System;

namespace BugDefender.OpenGL.BackgroundWorkers.NotificationBackroundWorker
{
    public class ManualDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ManualDefinition(string name, string description)
        {
            ID = Guid.NewGuid();
            Name = name;
            Description = description;
        }

        public ManualDefinition(Guid iD, string name, string description)
        {
            ID = iD;
            Name = name;
            Description = description;
        }
    }
}
