using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Enemies;
using TDGame.Core.Models;
using TDGame.Core.Turrets.Upgrades;
using static TDGame.Core.Enemies.EnemyDefinition;

namespace TDGame.Core.Turrets
{
    public enum TurretType { None, Laser, Projectile };
    public class TurretDefinition : BasePositionModel, ITextured
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TurretType Type { get; set; }
        public override float Size { get; set; }
        public int Cost { get; set; }
        public int Range { get; set; }
        public int Damage { get; set; }
        public int Cooldown { get; set; }
        public bool IsTrailing { get; set; }
        public string? ProjectileID { get; set; }
        public List<TurretLevel> TurretLevels { get; set; }
        public List<ProjectileLevel> ProjectileLevels { get; set; }
        public List<EnemyTypes> StrongAgainst { get; set; }
        public List<EnemyTypes> WeakAgainst { get; set; }

        [JsonIgnore]
        public TimeSpan CoolingFor { get; set; }
        [JsonIgnore]
        public EnemyDefinition? Targeting { get; set; }
        [JsonIgnore]
        public override float Angle { get; set; } = 0;
        [JsonIgnore]
        public int Kills { get; set; } = 0;

        public List<IUpgrade> GetAllUpgrades()
        {
            var returnList = new List<IUpgrade>();
            returnList.AddRange(TurretLevels);
            returnList.AddRange(ProjectileLevels);
            return returnList;
        }
    }
}
