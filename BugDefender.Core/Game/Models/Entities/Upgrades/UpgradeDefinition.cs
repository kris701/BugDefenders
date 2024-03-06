using BugDefender.Core.Game.Models.Entities.Turrets;
using System.Text;

namespace BugDefender.Core.Game.Models.Entities.Upgrades
{
    public class UpgradeDefinition : UpgradeEffectModel, IDefinition
    {
        public Guid ID { get; set; }
        public Guid? Requires { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }

        public UpgradeDefinition(Guid iD, Guid? requires, string name, string description, int cost, List<EffectTarget> effects) : base(effects)
        {
            ID = iD;
            Requires = requires;
            Name = name;
            Description = description;
            Cost = cost;
        }

        public void Apply(TurretInstance instance)
        {
            ApplyUpgradeEffectOnObject(instance.TurretInfo);
            instance.HasUpgrades.Add(ID);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(Description);
            foreach (var effect in Effects)
            {
                if (effect.Value != null)
                    sb.AppendLine($"{effect} {effect.Value}");
                else
                    sb.AppendLine($"{effect} {effect.Modifier}x");
            }

            return sb.ToString();
        }
    }
}
