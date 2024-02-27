using System.Text;
using BugDefender.Core.Game.Models.Entities.Projectiles.Modules;
using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.Entities.Turrets.Modules;

namespace BugDefender.Core.Game.Models.Entities.Upgrades
{
    public class DirectProjectileUpgrade : IUpgrade
    {
        public Guid ID { get; set; }
        public Guid? Requires { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public float DamageModifier { get; set; }
        public float SlowingFactorModifier { get; set; }
        public float SlowingDurationModifier { get; set; }

        public DirectProjectileUpgrade(Guid iD, Guid? requires, string name, string description, int cost, float damageModifier, float slowingFactorModifier, float slowingDurationModifier)
        {
            ID = iD;
            Requires = requires;
            Name = name;
            Description = description;
            Cost = cost;
            DamageModifier = damageModifier;
            SlowingFactorModifier = slowingFactorModifier;
            SlowingDurationModifier = slowingDurationModifier;
        }

        public void ApplyUpgrade(TurretInstance on)
        {
            if (on.TurretInfo is ProjectileTurretDefinition tur && tur.ProjectileInfo is DirectProjectileDefinition item)
            {
                item.Damage *= DamageModifier;
                item.SlowingFactor *= SlowingFactorModifier;
                item.SlowingDuration = (int)(item.SlowingDuration * SlowingDurationModifier);
                on.HasUpgrades.Add(ID);
            }
            else
                throw new Exception("Invalid upgrade type for turret!");
        }

        public string GetDescriptionString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Description);
            sb.AppendLine("Projectile get:");
            if (DamageModifier != 1)
                sb.AppendLine($"Damage {DamageModifier}x");
            if (SlowingFactorModifier != 1)
                sb.AppendLine($"Slowing Factor {SlowingFactorModifier}x");
            if (SlowingDurationModifier != 1)
                sb.AppendLine($"Slowing Duration {SlowingDurationModifier}x");

            return sb.ToString();
        }
    }
}
