using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using TDGame.Core.Models;
using TDGame.Core.Models.Entities.Upgrades;
namespace TDGame.Core.Models.Entities.Turrets
{
    public enum TurretType { None, Laser, Projectile, AOE };
    public class TurretDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TurretType Type { get; set; }
        public float Size { get; set; }
        public int Cost { get; set; }
        public float Range { get; set; }
        public float Damage { get; set; }
        public int Cooldown { get; set; }
        public bool IsTrailing { get; set; }
        public float SlowingFactor { get; set; }
        public int SlowingDuration { get; set; }
        public Guid? ProjectileID { get; set; }
        public List<TurretLevel> TurretLevels { get; set; }
        public List<ProjectileLevel> ProjectileLevels { get; set; }
        public List<DamageModifier> DamageModifiers { get; set; }

        public List<IUpgrade> GetAllUpgrades()
        {
            var returnList = new List<IUpgrade>();
            returnList.AddRange(TurretLevels);
            returnList.AddRange(ProjectileLevels);
            return returnList;
        }
    }
}
