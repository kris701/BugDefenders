using TDGame.Core.Game.Models;
using TDGame.Core.Users.Models.UserCriterias;

namespace TDGame.Core.Users.Models
{
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

        public bool IsValid<T>(UserDefinition<T> user)
        {
            foreach (var criteria in Criterias)
                if (!criteria.IsValid(user.Stats))
                    return false;
            return true;
        }
    }
}
