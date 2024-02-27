using System.Text;
using System.Text.Json.Serialization;
using TDGame.Core.Game.Models.Entities.Projectiles.Modules;
using TDGame.Core.Resources;

namespace TDGame.Core.Game.Models.Entities.Turrets.Modules
{
    public class ProjectileTurretDefinition : ITurretModule, IRangeAttribute, ICooldownAttribute
    {
        public float Range { get; set; }
        public int Cooldown { get; set; }
        public Guid ProjectileID { get; set; }
        public bool IsTrailing { get; set; }

        [JsonIgnore]
        public TimeSpan CoolingFor { get; set; }
        [JsonIgnore]
        public IProjectileModule ProjectileInfo { get; set; }

        public ProjectileTurretDefinition(float range, int cooldown, Guid projectileID, bool isTrailing)
        {
            Range = range;
            Cooldown = cooldown;
            ProjectileID = projectileID;
            IsTrailing = isTrailing;
            ProjectileInfo = ResourceManager.Projectiles.GetResource(ProjectileID).ModuleInfo.Copy();
        }

        public ITurretModule Copy()
        {
            return new ProjectileTurretDefinition(Range, Cooldown, ProjectileID, IsTrailing)
            {
                ProjectileInfo = ProjectileInfo.Copy()
            };
        }

        public string GetDescriptionString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Type: Projectile");
            if (Range != 0)
                sb.AppendLine($"Range: {Range}");
            sb.AppendLine($"Cooldown: {Cooldown}");
            sb.AppendLine();
            sb.AppendLine("Projectile:");
            sb.AppendLine(ProjectileInfo.GetDescriptionString());

            return sb.ToString();
        }
    }
}
