using BugDefender.Core.Game.Models;
using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models.UserCriterias;
using System.Text;

namespace BugDefender.Core.Users.Models.Buffs
{
    public class BuffDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? Requires { get; set; }

        public List<IUserCriteria> Criterias { get; set; }
        public BuffEffect Effect { get; set; }

        public BuffDefinition(Guid iD, string name, string description, Guid? requires, List<IUserCriteria> criterias, BuffEffect effect)
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

        public void Apply()
        {
            var buff = ResourceManager.Buffs.GetResource(ID).Effect;
            switch (buff.TargetType)
            {
                case BuffEffectTypes.Enemy: buff.ApplyUpgradeEffectOnObject(ResourceManager.Enemies.GetResource(buff.Target).ModuleInfo); break;
                case BuffEffectTypes.Turret: buff.ApplyUpgradeEffectOnObject(ResourceManager.Turrets.GetResource(buff.Target).ModuleInfo); break;
                case BuffEffectTypes.Projectile: buff.ApplyUpgradeEffectOnObject(ResourceManager.Projectiles.GetResource(buff.Target).ModuleInfo); break;
            }
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
