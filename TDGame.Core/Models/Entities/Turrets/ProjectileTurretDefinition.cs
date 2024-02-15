using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Models.Entities.Projectiles;
using TDGame.Core.Resources;
using static System.Net.Mime.MediaTypeNames;

namespace TDGame.Core.Models.Entities.Turrets
{
    public class ProjectileTurretDefinition : ITurretType
    {
        public float Range { get; set; }
        public int Cooldown { get; set; }
        public Guid ProjectileID { get; set; }
        public bool IsTrailing { get; set; }

        [JsonIgnore]
        public TimeSpan CoolingFor { get; set; }
        [JsonIgnore]
        public ProjectileDefinition ProjectileDefinition { get; set; }

        public ProjectileTurretDefinition(float range, int cooldown, Guid projectileID, bool isTrailing)
        {
            Range = range;
            Cooldown = cooldown;
            ProjectileID = projectileID;
            IsTrailing = isTrailing;
            ProjectileDefinition = ResourceManager.Projectiles.GetResource(ProjectileID).Copy();
        }

        public ITurretType Copy()
        {
            return new ProjectileTurretDefinition(Range, Cooldown, ProjectileID, IsTrailing);
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
            sb.AppendLine(ProjectileDefinition.GetDescriptionString());

            return sb.ToString();
        }
    }
}
