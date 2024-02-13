using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Enemies;
using TDGame.Core.Models;
using TDGame.Core.Turret;
using TDGame.Core.Turrets.Upgrades;

namespace TDGame.Core.Turrets
{
    public class TurretInstance : BasePositionModel, IInstance<TurretDefinition>
    {
        public Guid ID { get; set; }
        public Guid DefinitionID { get; set; }

        public float Range { get; set; }
        public float Damage { get; set; }
        public float Cooldown { get; set; }

        public TimeSpan CoolingFor { get; set; }
        public EnemyInstance? Targeting { get; set; }
        public int Kills { get; set; } = 0;
        public List<Guid> HasUpgrades { get; set; }
        public ProjectileDefinition? ProjectileDefinition { get; set; }

        public TurretInstance(Guid definitionID) : this(TurretBuilder.GetTurret(definitionID))
        {
        }

        public TurretInstance(TurretDefinition definition)
        {
            ID = Guid.NewGuid();
            DefinitionID = definition.ID;
            Range = definition.Range;
            Damage = definition.Damage;
            Cooldown = definition.Cooldown;
            Size = definition.Size;
            HasUpgrades = new List<Guid>();
            if (definition.ProjectileID != null)
                ProjectileDefinition = ProjectileBuilder.GetProjectile((Guid)definition.ProjectileID);
        }

        public TurretDefinition GetDefinition() => TurretBuilder.GetTurret(DefinitionID);

        public int GetTurretWorth()
        {
            var def = GetDefinition();
            var worth = def.Cost;
            foreach (var upgrade in def.GetAllUpgrades())
                if (HasUpgrades.Contains(upgrade.ID))
                    worth += upgrade.Cost;
            return worth;
        }

        public void ApplyUpgrade(Guid ID)
        {
            var def = GetDefinition();
            var upgrade = def.GetAllUpgrades().First(x => x.ID == ID);
            if (upgrade is TurretLevel turretLevel)
            {
                Range *= turretLevel.RangeModifier;
                Damage *= turretLevel.DamageModifier;
                Cooldown *= turretLevel.CooldownModifier;
                HasUpgrades.Add(turretLevel.ID);
            } else if (ProjectileDefinition != null && upgrade is ProjectileLevel projLev)
            {
                ProjectileDefinition.Damage *= projLev.DamageModifier;
                ProjectileDefinition.SplashRange *= projLev.SplashRangeModifier;
                ProjectileDefinition.TriggerRange *= projLev.TriggerRangeModifier;
                HasUpgrades.Add(projLev.ID);
            }
        }
    }
}
