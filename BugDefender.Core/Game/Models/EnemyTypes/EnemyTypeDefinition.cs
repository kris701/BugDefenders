using System.Text.Json.Serialization;

namespace BugDefender.Core.Game.Models.EnemyTypes
{
    public class EnemyTypeDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public EnemyTypeDefinition(Guid iD, string name, string description)
        {
            ID = iD;
            Name = name;
            Description = description;
        }
    }
}
