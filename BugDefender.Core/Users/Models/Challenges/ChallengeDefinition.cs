using BugDefender.Core.Game.Models;
using BugDefender.Core.Users.Models.UserCriterias;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Users.Models.Challenges
{
    public class ChallengeDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Reward { get; set; }

        public Guid MapID { get; set; }
        public Guid GameStyleID { get; set; }

        public List<IUserCriteria> Criterias { get; set; }

        public ChallengeDefinition(Guid iD, string name, string description, int reward, Guid mapID, Guid gameStyleID, List<IUserCriteria> criterias)
        {
            ID = iD;
            Name = name;
            Description = description;
            Reward = reward;
            MapID = mapID;
            GameStyleID = gameStyleID;
            Criterias = criterias;
        }

        public bool IsValid(StatsDefinition stats)
        {
            foreach (var criteria in Criterias)
                if (!criteria.IsValid(stats))
                    return false;
            return true;
        }
    }
}
