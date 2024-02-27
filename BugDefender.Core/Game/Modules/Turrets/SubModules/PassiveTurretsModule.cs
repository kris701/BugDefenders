using BugDefender.Core.Game.Helpers;
using BugDefender.Core.Game.Models.Entities.Projectiles.Modules;
using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.Entities.Turrets.Modules;

namespace BugDefender.Core.Game.Modules.Turrets.SubModules
{
    public class PassiveTurretsModule : BaseTurretModule<PassiveTurretDefinition>
    {
        private readonly Dictionary<TurretInstance, HashSet<TurretInstance>> _effectedTurrets = new Dictionary<TurretInstance, HashSet<TurretInstance>>();

        public PassiveTurretsModule(GameContext context, GameEngine game) : base(context, game)
        {
        }

        public override void Initialize()
        {
            Game.TurretsModule.OnTurretPurchased += TurretAdded;
            Game.TurretsModule.OnTurretSold += TurretRemoved;
            Game.TurretsModule.OnBeforeTurretUpgraded += TurretBeforeUpgraded;
            Game.TurretsModule.OnTurretUpgraded += TurretUpgraded;
        }

        private void UpdateEffectedList(PassiveTurretDefinition? unApply = null, PassiveTurretDefinition? apply = null)
        {
            foreach (var key in _effectedTurrets.Keys)
                if (key.TurretInfo is PassiveTurretDefinition def && unApply != def)
                    foreach (var effected in _effectedTurrets[key])
                        UnApplyEffect(def, effected);

            _effectedTurrets.Clear();
            foreach (var turret in Context.Turrets)
            {
                if (turret.TurretInfo is PassiveTurretDefinition def)
                {
                    _effectedTurrets.Add(turret, new HashSet<TurretInstance>());
                    foreach (var other in Context.Turrets)
                    {
                        if (other == turret || other.TurretInfo is PassiveTurretDefinition)
                            continue;
                        var dist = MathHelpers.Distance(turret, other);
                        if (dist < def.Range)
                            _effectedTurrets[turret].Add(other);
                    }
                }
            }

            foreach (var key in _effectedTurrets.Keys)
                if (key.TurretInfo is PassiveTurretDefinition def && apply != def)
                    foreach (var effected in _effectedTurrets[key])
                        ApplyEffect(def, effected);
        }

        private void TurretAdded(TurretInstance turret)
        {
            UpdateEffectedList();
        }

        private void TurretRemoved(TurretInstance turret)
        {
            if (turret.TurretInfo is PassiveTurretDefinition)
            {
                UpdateEffectedList();
            }
            else
            {
                foreach (var key in _effectedTurrets.Keys)
                    if (_effectedTurrets[key].Contains(turret))
                        _effectedTurrets[key].Remove(turret);
            }
        }

        private void TurretBeforeUpgraded(TurretInstance turret)
        {
            if (turret.TurretInfo is PassiveTurretDefinition def)
                foreach (var effected in _effectedTurrets[turret])
                    UnApplyEffect(def, effected);
        }

        private void TurretUpgraded(TurretInstance turret)
        {
            if (turret.TurretInfo is PassiveTurretDefinition def)
                UpdateEffectedList(def);
        }

        private void UnApplyEffect(PassiveTurretDefinition passive, TurretInstance turret)
        {
            switch (turret.TurretInfo)
            {
                case AOETurretDefinition def:
                    def.Damage /= passive.DamageModifier;
                    def.Range /= passive.RangeModifier;
                    def.Cooldown = (int)(def.Cooldown / passive.CooldownModifier);
                    def.SlowingFactor /= passive.SlowingFactorModifier;
                    def.SlowingDuration = def.SlowingDuration / passive.SlowingDurationModifier;
                    break;
                case LaserTurretDefinition def:
                    def.Damage /= passive.DamageModifier;
                    def.Range /= passive.RangeModifier;
                    def.Cooldown = (int)(def.Cooldown / passive.CooldownModifier);
                    def.SlowingFactor /= passive.SlowingFactorModifier;
                    def.SlowingDuration = def.SlowingDuration / passive.SlowingDurationModifier;
                    break;
                case ProjectileTurretDefinition def:
                    def.Range /= passive.RangeModifier;
                    def.Cooldown = (int)(def.Cooldown / passive.CooldownModifier);
                    UnApplyEffect(passive, def.ProjectileInfo);
                    break;
            }
        }

        private void UnApplyEffect(PassiveTurretDefinition passive, IProjectileModule projectile)
        {
            switch (projectile)
            {
                case DirectProjectileDefinition def:
                    def.Damage /= passive.DamageModifier;
                    def.SlowingFactor /= passive.SlowingFactorModifier;
                    def.SlowingDuration = def.SlowingDuration / passive.SlowingDurationModifier;
                    break;
                case ExplosiveProjectileDefinition def:
                    def.Damage /= passive.DamageModifier;
                    def.SlowingFactor /= passive.SlowingFactorModifier;
                    def.SlowingDuration = def.SlowingDuration / passive.SlowingDurationModifier;
                    break;
            }
        }

        private void ApplyEffect(PassiveTurretDefinition passive, TurretInstance turret)
        {
            switch (turret.TurretInfo)
            {
                case AOETurretDefinition def:
                    def.Damage *= passive.DamageModifier;
                    def.Range *= passive.RangeModifier;
                    def.Cooldown = (int)(def.Cooldown * passive.CooldownModifier);
                    def.SlowingFactor *= passive.SlowingFactorModifier;
                    def.SlowingDuration = def.SlowingDuration * passive.SlowingDurationModifier;
                    break;
                case LaserTurretDefinition def:
                    def.Damage *= passive.DamageModifier;
                    def.Range *= passive.RangeModifier;
                    def.Cooldown = (int)(def.Cooldown * passive.CooldownModifier);
                    def.SlowingFactor *= passive.SlowingFactorModifier;
                    def.SlowingDuration = def.SlowingDuration * passive.SlowingDurationModifier;
                    break;
                case ProjectileTurretDefinition def:
                    def.Range *= passive.RangeModifier;
                    def.Cooldown = (int)(def.Cooldown * passive.CooldownModifier);
                    ApplyEffect(passive, def.ProjectileInfo);
                    break;
            }
        }

        private void ApplyEffect(PassiveTurretDefinition passive, IProjectileModule projectile)
        {
            switch (projectile)
            {
                case DirectProjectileDefinition def:
                    def.Damage *= passive.DamageModifier;
                    def.SlowingFactor *= passive.SlowingFactorModifier;
                    def.SlowingDuration = def.SlowingDuration * passive.SlowingDurationModifier;
                    break;
                case ExplosiveProjectileDefinition def:
                    def.Damage *= passive.DamageModifier;
                    def.SlowingFactor *= passive.SlowingFactorModifier;
                    def.SlowingDuration = def.SlowingDuration * passive.SlowingDurationModifier;
                    break;
            }
        }

        public override void UpdateTurret(TimeSpan passed, TurretInstance turret, PassiveTurretDefinition def)
        {
        }
    }
}
