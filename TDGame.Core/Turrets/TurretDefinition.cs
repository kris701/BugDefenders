using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Enemies;
using TDGame.Core.Models;
using TDGame.Core.Turrets.Upgrades;

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
        public string? ProjectileID { get; set; }
        public List<TurretLevel> TurretLevels { get; set; }
        public List<ProjectileLevel> ProjectileLevels { get; set; }

        [JsonIgnore]
        public TimeSpan CoolingFor { get; set; }
        [JsonIgnore]
        public EnemyDefinition? Targeting { get; set; }
        [JsonIgnore]
        public override float Angle { get; set; } = 0;
        [JsonIgnore]
        public int Kills { get; set; } = 0;

        public TurretDefinition(Guid iD, string name, string description, TurretType type, float size, int cost, int range, int damage, int cooldown, string? projectileID, List<TurretLevel> turretLevels, List<ProjectileLevel> projectileLevels)
        {
            ID = iD;
            Name = name;
            Description = description;
            Type = type;
            Size = size;
            Cost = cost;
            Range = range;
            Damage = damage;
            Cooldown = cooldown;
            ProjectileID = projectileID;
            TurretLevels = turretLevels;
            ProjectileLevels = projectileLevels;
        }

        public List<IUpgrade> GetAllUpgrades()
        {
            var returnList = new List<IUpgrade>();
            returnList.AddRange(TurretLevels);
            returnList.AddRange(ProjectileLevels);
            return returnList;
        }
    }
}
