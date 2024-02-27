using System.Text;
using TDGame.Core.Game.Models;
using TDGame.Core.Users.Models.Buffs.BuffEffects;
using TDGame.Core.Users.Models.UserCriterias;

namespace TDGame.Core.Users.Models.Buffs
{
    public class BuffDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? Requires { get; set; }

        public List<IUserCriteria> Criterias { get; set; }
        public IBuffEffect Effect { get; set; }

        public BuffDefinition(Guid iD, string name, string description, Guid? requires, List<IUserCriteria> criterias, IBuffEffect effect)
        {
            ID = iD;
            Name = name;
            Description = description;
            Requires = requires;
            Criterias = criterias;
            Effect = effect;
        }

        public bool IsValid<T>(UserDefinition<T> user)
        {
            if (Requires != null && !user.Buffs.Contains((Guid)Requires))
                return false;
            foreach (var criteria in Criterias)
                if (!criteria.IsValid(user.Stats))
                    return false;
            return true;
        }

        public string GetDescriptionString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(Name);
            sb.AppendLine(Description);

            return sb.ToString();
        }
    }
}
