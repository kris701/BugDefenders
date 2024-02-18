using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Game.Models;
using TDGame.Core.Resources;
using TDGame.Core.Users.Models.Buffs;
using TDGame.Core.Users.Models.UpgradeRequirements;

namespace TDGame.Core.Users.Models
{
    public class BuffDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? Requires { get; set; }

        public IBuffCriteria Criteria { get; set; }
        public IBuff Effect { get; set; }

        public BuffDefinition(Guid iD, string name, string description, Guid? requires, IBuffCriteria criteria, IBuff effect)
        {
            ID = iD;
            Name = name;
            Description = description;
            Requires = requires;
            Criteria = criteria;
            Effect = effect;
            Effect.BuffID = ID;
        }

        public bool IsValid<T>(UserDefinition<T> user)
        {
            if (Requires != null && !user.Buffs.Contains((Guid)Requires))
                return false;
            return Criteria.IsValid(user.Stats);
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
