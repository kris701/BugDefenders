using BugDefender.Core.Game.Models;
using BugDefender.Core.Users.Models.UserCriterias;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Users.Models
{
    [JsonSerializable(typeof(AchivementDefinition))]
    public class AchivementDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<IUserCriteria> Criterias { get; set; }

        public AchivementDefinition(Guid iD, string name, string description, List<IUserCriteria> criterias)
        {
            ID = iD;
            Name = name;
            Description = description;
            Criterias = criterias;
        }

        public bool IsValid<T>(UserDefinition<T> user) where T : new()
        {
            foreach (var criteria in Criterias)
                if (!criteria.IsValid(user.Stats))
                    return false;
            return true;
        }
    }
}
