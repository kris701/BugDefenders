using BugDefender.Core.Game.Models;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Resources
{
    [JsonSerializable(typeof(ResourceDefinition))]
    public class ResourceDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public string Path { get; set; } = "";

        public ResourceDefinition(Guid iD, string version, string name, string description)
        {
            ID = iD;
            Version = version;
            Name = name;
            Description = description;
        }
    }
}
